namespace LifelogBb.Models.OAuth
{
    /// <summary>
    /// Rendered for the authorization errors that must not be redirected back to the client,
    /// because an unverified redirect URI cannot be trusted with the error.
    /// </summary>
    public class OAuthErrorViewModel
    {
        public string Error { get; set; } = "";

        public string Description { get; set; } = "";
    }
}
