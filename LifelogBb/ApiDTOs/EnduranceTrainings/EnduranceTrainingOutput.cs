using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.EnduranceTrainings
{
    public class EnduranceTrainingOutput : IBaseOutput
    {
        public long Id { get; set; }
        public string? Exercise { get; set; }

        public decimal Distance { get; set; }

        public TimeSpan? Duration { get; set; }

        public string? Notes { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
