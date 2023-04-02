using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Quotes
{
    public class QuoteInput
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
