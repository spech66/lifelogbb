using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Quotes
{
    public class EditQuoteViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
