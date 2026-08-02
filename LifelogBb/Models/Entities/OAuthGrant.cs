using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public enum OAuthGrantType
    {
        AuthorizationCode = 0,
        RefreshToken = 1
    }

    /// <summary>
    /// An issued authorization code or refresh token. Both kinds share a table so that revoking a
    /// whole lineage after a replay is a single delete on <see cref="SessionId"/>.
    /// </summary>
    public class OAuthGrant : BaseEntity
    {
        public OAuthGrantType GrantType { get; set; }

        public long OAuthClientId { get; set; }

        public OAuthClient? Client { get; set; }

        /// <summary>
        /// SHA-256 hex of the code or refresh token. The raw value is only ever seen by the client.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string TokenHash { get; set; } = "";

        /// <summary>
        /// Chains an authorization code to every refresh token descended from it. Reusing any token
        /// of the chain revokes all of them.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string SessionId { get; set; } = "";

        /// <summary>Authorization codes only. The token request has to present the same value.</summary>
        [StringLength(2000)]
        public string? RedirectUri { get; set; }

        /// <summary>Authorization codes only. PKCE S256 challenge (RFC 7636).</summary>
        [StringLength(200)]
        public string? CodeChallenge { get; set; }

        [Required]
        [StringLength(200)]
        public string Scope { get; set; } = "";

        /// <summary>Resource indicator (RFC 8707). Recorded for diagnostics, not enforced.</summary>
        [StringLength(2000)]
        public string? Resource { get; set; }

        public DateTime ExpiresAt { get; set; }

        /// <summary>Set when the grant is redeemed. A second attempt is treated as a replay.</summary>
        public DateTime? ConsumedAt { get; set; }
    }
}
