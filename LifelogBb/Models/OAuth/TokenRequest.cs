using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Models.OAuth
{
    /// <summary>
    /// Form encoded token request. There is no client authentication, clients are public and prove
    /// possession with PKCE.
    /// </summary>
    public class TokenRequest
    {
        [FromForm(Name = "grant_type")]
        public string? GrantType { get; set; }

        [FromForm(Name = "client_id")]
        public string? ClientId { get; set; }

        [FromForm(Name = "code")]
        public string? Code { get; set; }

        [FromForm(Name = "redirect_uri")]
        public string? RedirectUri { get; set; }

        [FromForm(Name = "code_verifier")]
        public string? CodeVerifier { get; set; }

        [FromForm(Name = "refresh_token")]
        public string? RefreshToken { get; set; }

        [FromForm(Name = "resource")]
        public string? Resource { get; set; }
    }
}
