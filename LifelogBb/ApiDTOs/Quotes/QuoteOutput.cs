using LifelogBb.Interfaces.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Quotes
{
    public class QuoteOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
