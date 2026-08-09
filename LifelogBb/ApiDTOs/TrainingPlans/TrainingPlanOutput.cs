using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.TrainingPlans
{
    public class TrainingPlanOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? Date { get; set; }

        public bool IsArchived { get; set; }

        public List<TrainingPlanSetOutput> Sets { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
