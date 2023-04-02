using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class Journal : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public Journal()
        {
            // Default constructor
        }

        public Journal(string text)
        {
            Text = text;
        }
    }
}
