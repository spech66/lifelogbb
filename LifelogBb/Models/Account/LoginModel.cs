using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Account
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        /// <summary>
        /// Where to go after a successful login. Set by the cookie handler when it intercepts a
        /// protected page, which is what brings the OAuth consent flow back to /oauth/authorize.
        /// Only local URLs are honored.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}
