using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Journals
{
    public class JournalInput
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
