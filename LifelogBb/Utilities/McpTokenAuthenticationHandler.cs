using LifelogBb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace LifelogBb.Utilities;

/// <summary>
/// Constants for the long lived MCP token authentication scheme.
/// </summary>
public static class McpTokenDefaults
{
    public const string AuthenticationScheme = "McpToken";

    public const string DisplayName = "MCP static token";

    /// <summary>
    /// Well known placeholder that never authenticates, mirroring the FeedToken default.
    /// </summary>
    public const string DisabledPlaceholder = "ChangeMeInTheConfig";
}

/// <summary>
/// Authenticates MCP clients with the long lived token from the app config
/// (<see cref="Models.Entities.Config.McpToken"/>) sent as "Authorization: Bearer &lt;token&gt;".
/// Falls back to the regular JWT bearer scheme so existing API tokens keep working on /mcp.
/// </summary>
public class McpTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string BearerPrefix = "Bearer ";

    private readonly LifelogBbContext _context;

    public McpTokenAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, LifelogBbContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authorization = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var presentedToken = authorization[BearerPrefix.Length..].Trim();
        if (presentedToken.Length == 0)
        {
            return AuthenticateResult.NoResult();
        }

        // Read only. Config.GetConfig would insert and save a row as a side effect which must not
        // happen while the request is still unauthenticated.
        var configuredToken = await _context.Configs.AsNoTracking().Select(c => c.McpToken).FirstOrDefaultAsync();

        if (IsEnabled(configuredToken) && FixedTimeEquals(configuredToken!, presentedToken))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "MCP client"),
                new Claim(ClaimTypes.Role, "Administrator"),
            };

            // The authentication type has to be set or ClaimsIdentity.IsAuthenticated stays false.
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
        }

        // Not the configured token (or token authentication is disabled), try the regular JWT flow.
        return await Context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // Never redirect to the cookie login page, MCP clients need a plain 401.
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers[HeaderNames.WWWAuthenticate] = "Bearer";
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    private static bool IsEnabled(string? configuredToken) =>
        !string.IsNullOrWhiteSpace(configuredToken) && configuredToken != McpTokenDefaults.DisabledPlaceholder;

    private static bool FixedTimeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
