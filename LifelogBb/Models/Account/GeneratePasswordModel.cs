using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Account
{
    public class GeneratePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? PasswordHash { get; set; }
    }
}
