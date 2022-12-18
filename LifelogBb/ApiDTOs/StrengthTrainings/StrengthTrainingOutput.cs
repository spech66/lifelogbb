using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.StrengthTrainings
{
    public class StrengthTrainingOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Exercise { get; set; } = string.Empty;

        public int Reps { get; set; }

        public double Weight { get; set; }

        public string? Notes { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
