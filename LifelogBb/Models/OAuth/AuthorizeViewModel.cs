using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifelogBb.Models.OAuth
{
    /// <summary>
    /// The authorization request, bound from the query string on GET and from the consent form on
    /// POST. ModelBinder only renames the parameters, it does not pin them to a single source, so
    /// the same model serves both. Everything here is client supplied and is revalidated against
    /// the database on both requests.
    /// </summary>
    public class AuthorizeViewModel
    {
        [ModelBinder(Name = "response_type")]
        public string? ResponseType { get; set; }

        [ModelBinder(Name = "client_id")]
        public string? ClientId { get; set; }

        [ModelBinder(Name = "redirect_uri")]
        public string? RedirectUri { get; set; }

        [ModelBinder(Name = "code_challenge")]
        public string? CodeChallenge { get; set; }

        [ModelBinder(Name = "code_challenge_method")]
        public string? CodeChallengeMethod { get; set; }

        [ModelBinder(Name = "state")]
        public string? State { get; set; }

        [ModelBinder(Name = "scope")]
        public string? Scope { get; set; }

        [ModelBinder(Name = "resource")]
        public string? Resource { get; set; }

        /// <summary>Display only, always resolved from the database. Never taken from the form.</summary>
        [BindNever]
        public string ClientName { get; set; } = "";

        /// <summary>The scope that will actually be granted, which is always the single app scope.</summary>
        [BindNever]
        public string GrantedScope { get; set; } = "";
    }
}
