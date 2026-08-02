namespace LifelogBb.Utilities;

/// <summary>
/// Builds the externally visible base URL. OAuth metadata has to advertise the URL the client
/// actually used, not the one Kestrel is bound to.
/// </summary>
public static class PublicUrl
{
    /// <summary>
    /// Escape hatch for reverse proxies that do not forward the scheme or host correctly.
    /// Normally unset, because app.UseForwardedHeaders() already fixes both.
    /// </summary>
    public const string OverrideConfigKey = "Authentication:OAuth:PublicBaseUrl";

    /// <summary>Returns the base URL without a trailing slash, e.g. "https://lifelog.example".</summary>
    public static string GetBaseUrl(HttpRequest request, IConfiguration? configuration = null)
    {
        var configured = configuration?[OverrideConfigKey];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.TrimEnd('/');
        }

        return $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
    }
}
