using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LifelogBb.ApiDTOs.Journals
{
    public class JournalInput
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Tags { get; set; }

        [JsonRequired]
        [Range(typeof(DateTime), "0001-01-02", "9999-12-31")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
