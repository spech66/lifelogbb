using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LifelogBb.ApiDTOs.Journals
{
    public class JournalInput : IValidatableObject
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Tags { get; set; }

        [Required]
        [JsonRequired]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == DateTime.MinValue)
            {
                yield return new ValidationResult("Date is required.", new[] { nameof(Date) });
            }
        }
    }
}
