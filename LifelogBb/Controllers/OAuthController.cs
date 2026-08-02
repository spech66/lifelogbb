using LifelogBb.Models;
using LifelogBb.Models.Entities;
using LifelogBb.Models.OAuth;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Westwind.AspNetCore.Security;

namespace LifelogBb.Controllers
{
    /// <summary>
    /// A minimal OAuth 2.1 authorization server for MCP clients that cannot use the static MCP
    /// token, most notably the Claude connectors. It covers protected resource metadata (RFC 9728),
    /// authorization server metadata (RFC 8414), dynamic client registration (RFC 7591) and the
    /// authorization code flow with mandatory PKCE.
    ///
    /// The resource owner is the single app password, so the authorization endpoint simply reuses
    /// the normal cookie login. Access tokens are the same JWTs /api/authentication issues, which is
    /// why no change to the /mcp authentication is needed to accept them.
    ///
    /// AllowAnonymous is deliberately per action instead of on the class: any IAllowAnonymous in the
    /// endpoint metadata would also switch off the Authorize attribute on the authorize endpoints.
    /// </summary>
    [RequireOAuthEnabled]
    public class OAuthController : Controller
    {
        private const string CookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>Throttles the opportunistic cleanup of expired grants. Process wide.</summary>
        private static long _lastSweepTicks;

        private readonly LifelogBbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OAuthController> _logger;

        public OAuthController(LifelogBbContext context, IConfiguration configuration, ILogger<OAuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        #region Discovery

        /// <summary>
        /// Protected resource metadata (RFC 9728). Clients find this through the resource_metadata
        /// parameter of the 401 challenge on /mcp. The catch all serves both the bare path and the
        /// "/mcp" suffixed variant clients derive from the resource path.
        /// </summary>
        [HttpGet("~/.well-known/oauth-protected-resource")]
        [HttpGet("~/.well-known/oauth-protected-resource/{*resourcePath}")]
        [AllowAnonymous]
        public IActionResult ProtectedResourceMetadata(string? resourcePath)
        {
            if (resourcePath is not (null or "mcp"))
            {
                return NotFound();
            }

            var baseUrl = PublicUrl.GetBaseUrl(Request, _configuration);
            return MetadataJson(new
            {
                resource = $"{baseUrl}{OAuthDefaults.ResourcePath}",
                authorization_servers = new[] { baseUrl },
                scopes_supported = new[] { OAuthDefaults.Scope },
                bearer_methods_supported = new[] { "header" },
                resource_name = "LifelogBB MCP"
            });
        }

        /// <summary>
        /// Authorization server metadata (RFC 8414). The issuer has to be byte for byte the base URL
        /// the document was fetched from, so it is derived from the request and never from the JWT
        /// issuer setting, which describes the token and not this endpoint.
        /// </summary>
        [HttpGet("~/.well-known/oauth-authorization-server")]
        [HttpGet("~/.well-known/oauth-authorization-server/{*issuerPath}")]
        [AllowAnonymous]
        public IActionResult AuthorizationServerMetadata(string? issuerPath)
        {
            if (issuerPath is not (null or "mcp"))
            {
                return NotFound();
            }

            var baseUrl = PublicUrl.GetBaseUrl(Request, _configuration);
            return MetadataJson(new
            {
                issuer = baseUrl,
                authorization_endpoint = $"{baseUrl}/oauth/authorize",
                token_endpoint = $"{baseUrl}/oauth/token",
                registration_endpoint = $"{baseUrl}/oauth/register",
                scopes_supported = new[] { OAuthDefaults.Scope },
                response_types_supported = new[] { "code" },
                response_modes_supported = new[] { "query" },
                grant_types_supported = new[] { "authorization_code", "refresh_token" },
                token_endpoint_auth_methods_supported = new[] { "none" },
                code_challenge_methods_supported = new[] { "S256" }
            });
        }

        private IActionResult MetadataJson(object metadata)
        {
            Response.Headers.CacheControl = "public, max-age=300";
            return Json(metadata);
        }

        #endregion

        #region Dynamic client registration

        /// <summary>
        /// Dynamic client registration (RFC 7591). Only public clients are accepted, so no secret is
        /// ever issued or stored. Anyone who can reach the instance can register, but a registration
        /// on its own grants nothing: a token still requires the app password at the consent step.
        ///
        /// No antiforgery token, and there cannot be one: the caller is a non browser HTTP client with
        /// no session and no way to obtain one. Nothing here acts on an existing session either, so
        /// there is no cross site request to forge.
        /// </summary>
        [HttpPost("~/oauth/register")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [RequestSizeLimit(16 * 1024)]
        public async Task<IActionResult> Register([FromBody] ClientRegistrationRequest request)
        {
            if (request?.RedirectUris is null || request.RedirectUris.Length == 0)
            {
                return OAuthError("invalid_client_metadata", "redirect_uris is required.");
            }

            if (request.RedirectUris.Length > OAuthDefaults.MaxRedirectUrisPerClient)
            {
                return OAuthError("invalid_client_metadata", $"At most {OAuthDefaults.MaxRedirectUrisPerClient} redirect URIs are supported.");
            }

            foreach (var redirectUri in request.RedirectUris)
            {
                if (!IsAllowedRedirectUri(redirectUri))
                {
                    return OAuthError("invalid_redirect_uri", $"Unsupported redirect URI '{redirectUri}'. Use https, or http on a loopback host.");
                }
            }

            if (!string.IsNullOrEmpty(request.TokenEndpointAuthMethod) && request.TokenEndpointAuthMethod != "none")
            {
                return OAuthError("invalid_client_metadata", "Only public clients are supported, token_endpoint_auth_method must be 'none'.");
            }

            if (request.GrantTypes is not null &&
                request.GrantTypes.Any(g => g is not ("authorization_code" or "refresh_token")))
            {
                return OAuthError("invalid_client_metadata", "Only the authorization_code and refresh_token grants are supported.");
            }

            if (request.ResponseTypes is not null && request.ResponseTypes.Any(r => r != "code"))
            {
                return OAuthError("invalid_client_metadata", "Only the 'code' response type is supported.");
            }

            var clientName = string.IsNullOrWhiteSpace(request.ClientName)
                ? "MCP client"
                : request.ClientName.Trim();
            if (clientName.Length > OAuthDefaults.MaxClientNameLength)
            {
                clientName = clientName[..OAuthDefaults.MaxClientNameLength];
            }

            var redirectUris = string.Join('\n', request.RedirectUris.Select(u => u.Trim()));

            // Clients re-register on every reconnect. Returning the existing record keeps the table
            // from growing one row per connection attempt.
            var existing = await _context.OAuthClients
                .FirstOrDefaultAsync(c => c.RedirectUris == redirectUris && c.ClientName == clientName);
            if (existing is not null)
            {
                return Json(BuildRegistrationResponse(existing));
            }

            var unusedCutoff = DateTime.UtcNow - OAuthDefaults.UnusedClientLifetime;
            await _context.OAuthClients
                .Where(c => c.LastUsedAt == null && c.CreatedAt < unusedCutoff)
                .ExecuteDeleteAsync();

            if (await _context.OAuthClients.CountAsync() >= OAuthDefaults.MaxClients)
            {
                return OAuthError("invalid_client_metadata", "Client registration limit reached.");
            }

            var client = new OAuthClient
            {
                ClientId = OAuthCrypto.NewSecret(16),
                ClientName = clientName,
                RedirectUris = redirectUris,
                Scope = OAuthDefaults.Scope
            };
            client.SetCreateFields();

            _context.OAuthClients.Add(client);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Registered OAuth client {ClientName} ({ClientId}).",
                LogSanitizer.ForLog(client.ClientName), client.ClientId);

            return StatusCode(StatusCodes.Status201Created, BuildRegistrationResponse(client));
        }

        private static object BuildRegistrationResponse(OAuthClient client) => new
        {
            client_id = client.ClientId,
            client_id_issued_at = new DateTimeOffset(client.CreatedAt, TimeSpan.Zero).ToUnixTimeSeconds(),
            client_name = client.ClientName,
            redirect_uris = client.RedirectUriList,
            grant_types = new[] { "authorization_code", "refresh_token" },
            response_types = new[] { "code" },
            token_endpoint_auth_method = "none",
            scope = client.Scope
        };

        /// <summary>
        /// https only, except on a loopback host where a local client cannot get a certificate.
        /// Fragments are rejected because a redirect URI must not carry one.
        /// </summary>
        private static bool IsAllowedRedirectUri(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > OAuthDefaults.MaxUriLength)
            {
                return false;
            }

            if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) || !string.IsNullOrEmpty(uri.Fragment))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback);
        }

        #endregion

        #region Authorization endpoint

        /// <summary>
        /// The consent screen.
        /// </summary>
        [HttpGet("~/oauth/authorize")]
        [AllowAnonymous]
        public async Task<IActionResult> Authorize(AuthorizeViewModel model)
        {
            var challenge = await RequireInteractiveLoginAsync();
            if (challenge is not null)
            {
                return challenge;
            }

            var (client, failure) = await ResolveClientAsync(model);
            if (failure is not null)
            {
                return failure;
            }

            var invalid = ValidateAuthorizeRequest(model);
            if (invalid is not null)
            {
                return invalid;
            }

            model.ClientName = client!.ClientName;
            model.GrantedScope = OAuthDefaults.Scope;
            return View(model);
        }

        [HttpPost("~/oauth/authorize")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(AuthorizeViewModel model, string decision)
        {
            var challenge = await RequireInteractiveLoginAsync();
            if (challenge is not null)
            {
                return challenge;
            }

            // The hidden fields are attacker controllable, so the full validation runs again.
            var (client, failure) = await ResolveClientAsync(model);
            if (failure is not null)
            {
                return failure;
            }

            var invalid = ValidateAuthorizeRequest(model);
            if (invalid is not null)
            {
                return invalid;
            }

            if (decision != "approve")
            {
                return RedirectWithError(model, "access_denied", "The user denied the request.");
            }

            var code = OAuthCrypto.NewSecret();
            var grant = new OAuthGrant
            {
                GrantType = OAuthGrantType.AuthorizationCode,
                OAuthClientId = client!.Id,
                TokenHash = OAuthCrypto.Sha256Hex(code),
                SessionId = OAuthCrypto.NewSecret(16),
                RedirectUri = model.RedirectUri,
                CodeChallenge = model.CodeChallenge,
                Scope = OAuthDefaults.Scope,
                Resource = model.Resource,
                ExpiresAt = DateTime.UtcNow.Add(OAuthDefaults.CodeLifetime)
            };
            grant.SetCreateFields();
            _context.OAuthGrants.Add(grant);

            client.LastUsedAt = DateTime.UtcNow;
            client.SetUpdateFields();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Issued an authorization code to {ClientName} ({ClientId}).",
                LogSanitizer.ForLog(client.ClientName), client.ClientId);

            return Redirect(QueryHelpers.AddQueryString(model.RedirectUri!, new Dictionary<string, string?>
            {
                ["code"] = code,
                ["state"] = model.State
            }));
        }

        /// <summary>
        /// Only a real browser session may grant consent. The check happens here rather than through
        /// an Authorize attribute for two reasons: attribute metadata is handled by the authorization
        /// middleware, which would redirect to the login page before RequireOAuthEnabled can hide the
        /// endpoint, and authenticating the cookie scheme explicitly stops a bearer token from
        /// approving a fresh grant for itself.
        /// </summary>
        private async Task<IActionResult?> RequireInteractiveLoginAsync()
        {
            var result = await HttpContext.AuthenticateAsync(CookieScheme);
            if (result.Succeeded)
            {
                return null;
            }

            // The cookie handler builds the login redirect with the current URL as ReturnUrl, which is
            // what brings the browser back to this authorization request after signing in.
            return Challenge(CookieScheme);
        }

        /// <summary>
        /// Resolves the client and pins down the redirect URI. These two failures must never be
        /// redirected: without a verified redirect URI there is nowhere trustworthy to send them.
        /// </summary>
        private async Task<(OAuthClient? Client, IActionResult? Failure)> ResolveClientAsync(AuthorizeViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ClientId))
            {
                return (null, ErrorView("invalid_client", "The client_id parameter is missing."));
            }

            var client = await _context.OAuthClients.FirstOrDefaultAsync(c => c.ClientId == model.ClientId);
            if (client is null)
            {
                return (null, ErrorView("invalid_client", "Unknown client_id. The client has to register again."));
            }

            var registered = client.RedirectUriList;
            if (string.IsNullOrWhiteSpace(model.RedirectUri))
            {
                if (registered.Count != 1)
                {
                    return (null, ErrorView("invalid_request", "The redirect_uri parameter is required for this client."));
                }

                model.RedirectUri = registered[0];
            }
            else if (!registered.Contains(model.RedirectUri, StringComparer.Ordinal))
            {
                return (null, ErrorView("invalid_redirect_uri", "The redirect_uri is not registered for this client."));
            }

            return (client, null);
        }

        /// <summary>
        /// Everything the client can be told about by redirecting back to its verified redirect URI.
        /// </summary>
        private IActionResult? ValidateAuthorizeRequest(AuthorizeViewModel model)
        {
            if (model.ResponseType != "code")
            {
                return RedirectWithError(model, "unsupported_response_type", "Only the 'code' response type is supported.");
            }

            if (string.IsNullOrEmpty(model.CodeChallenge))
            {
                return RedirectWithError(model, "invalid_request", "PKCE is required, code_challenge is missing.");
            }

            // RFC 7636 treats a missing method as 'plain'. Rejecting it explicitly keeps a downgrade
            // from silently succeeding.
            if (model.CodeChallengeMethod != "S256")
            {
                return RedirectWithError(model, "invalid_request", "code_challenge_method must be S256.");
            }

            if (!OAuthCrypto.IsValidPkceValue(model.CodeChallenge))
            {
                return RedirectWithError(model, "invalid_request", "The code_challenge is malformed.");
            }

            if (model.State is { Length: > OAuthDefaults.MaxStateLength })
            {
                return RedirectWithError(model, "invalid_request", "The state parameter is too long.");
            }

            var expectedResource = $"{PublicUrl.GetBaseUrl(Request, _configuration)}{OAuthDefaults.ResourcePath}";
            if (!string.IsNullOrEmpty(model.Resource) && model.Resource != expectedResource)
            {
                // Recorded but not enforced, see the audience note in the README.
                // Both values are attacker reachable: the resource comes straight from the query and
                // the expected value is built from the Host header.
                _logger.LogWarning("Authorization request asked for resource {Resource}, expected {Expected}.",
                    LogSanitizer.ForLog(model.Resource), LogSanitizer.ForLog(expectedResource));
            }

            return null;
        }

        private IActionResult RedirectWithError(AuthorizeViewModel model, string error, string description) =>
            Redirect(QueryHelpers.AddQueryString(model.RedirectUri!, new Dictionary<string, string?>
            {
                ["error"] = error,
                ["error_description"] = description,
                ["state"] = model.State
            }));

        private IActionResult ErrorView(string error, string description)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return View("Error", new OAuthErrorViewModel { Error = error, Description = description });
        }

        #endregion

        #region Token endpoint

        /// <summary>
        /// No antiforgery token here: the endpoint is unauthenticated and its security comes from the
        /// authorization code plus the PKCE verifier, neither of which an attacker's page can supply.
        /// </summary>
        [HttpPost("~/oauth/token")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        [RequestSizeLimit(16 * 1024)]
        public async Task<IActionResult> Token([FromForm] TokenRequest form)
        {
            Response.Headers.CacheControl = "no-store";
            Response.Headers.Pragma = "no-cache";

            await SweepExpiredGrantsAsync();

            return form.GrantType switch
            {
                "authorization_code" => await ExchangeAuthorizationCodeAsync(form),
                "refresh_token" => await ExchangeRefreshTokenAsync(form),
                _ => OAuthError("unsupported_grant_type", "Only authorization_code and refresh_token are supported.")
            };
        }

        private async Task<IActionResult> ExchangeAuthorizationCodeAsync(TokenRequest form)
        {
            if (string.IsNullOrEmpty(form.Code))
            {
                return OAuthError("invalid_request", "The code parameter is missing.");
            }

            var grant = await _context.OAuthGrants
                .Include(g => g.Client)
                .FirstOrDefaultAsync(g => g.TokenHash == OAuthCrypto.Sha256Hex(form.Code)
                                       && g.GrantType == OAuthGrantType.AuthorizationCode);

            if (grant is null)
            {
                return OAuthError("invalid_grant", "Unknown authorization code.");
            }

            if (grant.ConsumedAt is not null)
            {
                return await RevokeLineageAsync(grant, "The authorization code was already used.");
            }

            // Burn the code before anything else so two concurrent requests cannot both redeem it.
            grant.ConsumedAt = DateTime.UtcNow;
            grant.SetUpdateFields();
            await _context.SaveChangesAsync();

            if (grant.ExpiresAt < DateTime.UtcNow)
            {
                return OAuthError("invalid_grant", "The authorization code expired.");
            }

            if (grant.Client!.ClientId != form.ClientId)
            {
                return OAuthError("invalid_grant", "The authorization code was issued to a different client.");
            }

            if (!string.Equals(grant.RedirectUri, form.RedirectUri, StringComparison.Ordinal))
            {
                return OAuthError("invalid_grant", "The redirect_uri does not match the authorization request.");
            }

            if (!OAuthCrypto.IsValidPkceValue(form.CodeVerifier) ||
                !OAuthCrypto.VerifyPkceS256(form.CodeVerifier!, grant.CodeChallenge!))
            {
                return OAuthError("invalid_grant", "PKCE verification failed.");
            }

            return await IssueTokensAsync(grant.Client, grant.SessionId, grant.Resource);
        }

        private async Task<IActionResult> ExchangeRefreshTokenAsync(TokenRequest form)
        {
            if (string.IsNullOrEmpty(form.RefreshToken))
            {
                return OAuthError("invalid_request", "The refresh_token parameter is missing.");
            }

            var grant = await _context.OAuthGrants
                .Include(g => g.Client)
                .FirstOrDefaultAsync(g => g.TokenHash == OAuthCrypto.Sha256Hex(form.RefreshToken)
                                       && g.GrantType == OAuthGrantType.RefreshToken);

            if (grant is null)
            {
                return OAuthError("invalid_grant", "Unknown refresh token.");
            }

            if (grant.ConsumedAt is not null)
            {
                return await RevokeLineageAsync(grant, "The refresh token was already used.");
            }

            if (grant.ExpiresAt < DateTime.UtcNow)
            {
                return OAuthError("invalid_grant", "The refresh token expired.");
            }

            if (grant.Client!.ClientId != form.ClientId)
            {
                return OAuthError("invalid_grant", "The refresh token was issued to a different client.");
            }

            grant.ConsumedAt = DateTime.UtcNow;
            grant.SetUpdateFields();
            await _context.SaveChangesAsync();

            return await IssueTokensAsync(grant.Client, grant.SessionId, grant.Resource);
        }

        /// <summary>
        /// Replaying any grant of a chain means either the client is broken or a token leaked, so the
        /// whole chain goes (RFC 9700 refresh token reuse detection).
        /// </summary>
        private async Task<IActionResult> RevokeLineageAsync(OAuthGrant grant, string description)
        {
            _logger.LogWarning("Replayed OAuth grant for session {SessionId}, revoking the whole chain.", grant.SessionId);
            await _context.OAuthGrants.Where(g => g.SessionId == grant.SessionId).ExecuteDeleteAsync();
            return OAuthError("invalid_grant", description);
        }

        private async Task<IActionResult> IssueTokensAsync(OAuthClient client, string sessionId, string? resource)
        {
            var refreshToken = OAuthCrypto.NewSecret();
            var refreshGrant = new OAuthGrant
            {
                GrantType = OAuthGrantType.RefreshToken,
                OAuthClientId = client.Id,
                TokenHash = OAuthCrypto.Sha256Hex(refreshToken),
                SessionId = sessionId,
                Scope = OAuthDefaults.Scope,
                Resource = resource,
                ExpiresAt = DateTime.UtcNow.Add(OAuthDefaults.RefreshLifetime)
            };
            refreshGrant.SetCreateFields();
            _context.OAuthGrants.Add(refreshGrant);

            client.LastUsedAt = DateTime.UtcNow;
            client.SetUpdateFields();

            await _context.SaveChangesAsync();

            // Same claims and settings as /api/authentication, so the already configured JwtBearer
            // handler validates these tokens on /mcp without any extra wiring.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Default user"),
                new Claim(ClaimTypes.Role, "Administrator"),
            };

            var lifetime = TimeSpan.FromMinutes(double.Parse(_configuration["Authentication:JwtToken:TokenTimeoutMinutes"]!));
            var token = JwtHelper.GetJwtToken(
                "Default user",
                _configuration["Authentication:JwtToken:SigningKey"]!,
                _configuration["Authentication:JwtToken:Issuer"]!,
                _configuration["Authentication:JwtToken:Audience"]!,
                lifetime,
                claims.ToArray()
            );

            return Json(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "Bearer",
                expires_in = (int)lifetime.TotalSeconds,
                refresh_token = refreshToken,
                scope = OAuthDefaults.Scope
            });
        }

        /// <summary>
        /// Cheap cleanup on the token endpoint instead of a hosted service. Consumed and expired rows
        /// are kept for a day so a replay is still recognised rather than looking like an unknown code.
        /// </summary>
        private async Task SweepExpiredGrantsAsync()
        {
            var now = DateTime.UtcNow.Ticks;
            var last = Interlocked.Read(ref _lastSweepTicks);
            if (last != 0 && now - last < OAuthDefaults.SweepInterval.Ticks)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _lastSweepTicks, now, last) != last)
            {
                return;
            }

            var cutoff = DateTime.UtcNow - OAuthDefaults.ExpiredGrantRetention;
            var removed = await _context.OAuthGrants.Where(g => g.ExpiresAt < cutoff).ExecuteDeleteAsync();
            if (removed > 0)
            {
                _logger.LogInformation("Removed {Count} expired OAuth grants.", removed);
            }
        }

        #endregion

        private IActionResult OAuthError(string error, string? description = null, int statusCode = StatusCodes.Status400BadRequest) =>
            StatusCode(statusCode, new { error, error_description = description });
    }
}
