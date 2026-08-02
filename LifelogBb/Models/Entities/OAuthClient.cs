using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    /// <summary>
    /// A public OAuth client created through dynamic client registration (RFC 7591).
    /// There are no client secrets, every client authenticates with PKCE only.
    /// </summary>
    public class OAuthClient : BaseEntity
    {
        [Required]
        [StringLength(64)]
        public string ClientId { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string ClientName { get; set; } = "";

        /// <summary>
        /// Newline separated redirect URIs. Compared with exact ordinal matching and never queried
        /// by URI, so a separate table would only add joins.
        /// </summary>
        [Required]
        public string RedirectUris { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string Scope { get; set; } = "";

        /// <summary>
        /// Set the first time the client completes an authorization. Registrations that never get
        /// used are pruned.
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        [NotMapped]
        public IReadOnlyList<string> RedirectUriList =>
            RedirectUris.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        public ICollection<OAuthGrant> Grants { get; set; } = new List<OAuthGrant>();
    }
}
