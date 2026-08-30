using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.TrainingPlans
{
    public class TrainingPlanSetOutput : IBaseOutput
    {
        public long Id { get; set; }

        public long TrainingPlanId { get; set; }

        public string Exercise { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public int Reps { get; set; }

        public double? Weight { get; set; }

        public int? DurationSeconds { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
