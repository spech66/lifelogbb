using System.Text.Json.Serialization;

namespace LifelogBb.Models.OAuth
{
    /// <summary>
    /// Dynamic client registration request (RFC 7591). Only the members we act on are declared,
    /// everything else in the document is ignored as the spec allows.
    /// </summary>
    public class ClientRegistrationRequest
    {
        [JsonPropertyName("redirect_uris")]
        public string[]? RedirectUris { get; set; }

        [JsonPropertyName("client_name")]
        public string? ClientName { get; set; }

        [JsonPropertyName("grant_types")]
        public string[]? GrantTypes { get; set; }

        [JsonPropertyName("response_types")]
        public string[]? ResponseTypes { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("token_endpoint_auth_method")]
        public string? TokenEndpointAuthMethod { get; set; }
    }
}
