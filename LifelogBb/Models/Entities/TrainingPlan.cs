using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class TrainingPlan : BaseEntity
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Null = base/template plan, reusable as a starting point.
        // Set = a concrete day plan, meant to be worked through once.
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        public bool IsArchived { get; set; }

        public ICollection<TrainingPlanSet> Sets { get; set; } = new List<TrainingPlanSet>();
    }
}
