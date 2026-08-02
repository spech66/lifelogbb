namespace LifelogBb.Utilities;

/// <summary>
/// Constants for the built-in OAuth 2.1 authorization server used by MCP clients.
/// </summary>
public static class OAuthDefaults
{
    /// <summary>The only scope. It always grants full access, mirroring the single user model.</summary>
    public const string Scope = "lifelogbb";

    /// <summary>Path of the protected resource the tokens are meant for.</summary>
    public const string ResourcePath = "/mcp";

    public const string ProtectedResourceMetadataPath = "/.well-known/oauth-protected-resource";

    public const string AuthorizationServerMetadataPath = "/.well-known/oauth-authorization-server";

    /// <summary>Authorization codes are exchanged immediately, so this can be short.</summary>
    public static readonly TimeSpan CodeLifetime = TimeSpan.FromSeconds(60);

    public static readonly TimeSpan RefreshLifetime = TimeSpan.FromDays(30);

    /// <summary>Registrations that never completed a flow are pruned after this long.</summary>
    public static readonly TimeSpan UnusedClientLifetime = TimeSpan.FromDays(1);

    /// <summary>Expired grants are deleted this long after they expired.</summary>
    public static readonly TimeSpan ExpiredGrantRetention = TimeSpan.FromDays(1);

    public static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(10);

    public const int MaxClients = 100;
    public const int MaxRedirectUrisPerClient = 10;
    public const int MaxUriLength = 2000;
    public const int MaxStateLength = 512;
    public const int MaxClientNameLength = 200;

    /// <summary>RFC 7636 bounds for both the code verifier and the code challenge.</summary>
    public const int MinPkceLength = 43;
    public const int MaxPkceLength = 128;
}
