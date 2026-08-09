using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class TrainingPlanSet : BaseEntity
    {
        [Required]
        public long TrainingPlanId { get; set; }

        [Required]
        [MinLength(1)]
        public string Exercise { get; set; } = string.Empty;

        // Position of this set within the plan. Assigned from array order on Create/Update, not user-editable.
        public int SortOrder { get; set; }

        public int Reps { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Weight { get; set; }

        public string? Notes { get; set; }

        public TrainingPlan TrainingPlan { get; set; } = null!;
    }
}
