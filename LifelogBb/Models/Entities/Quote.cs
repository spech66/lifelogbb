using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class Quote : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public Quote()
        {
            // Default constructor
        }

        public Quote(string text, string author)
        {
            Text = text;
            Author = author;
        }
    }
}
