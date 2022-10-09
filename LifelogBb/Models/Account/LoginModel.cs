using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Account
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
