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

        // 0 for pure holds, where DurationSeconds carries the effort instead.
        public int Reps { get; set; }

        // Null means no weight applies (bodyweight, band, mobility work) as opposed to an actual 0.
        // Volume aggregations skip null sets instead of dragging the average down with zero volume.
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? Weight { get; set; }

        // Set for timed work (planks, stretches, holds) where reps do not describe the effort.
        // Combined with Reps it reads as "3 reps of 30 s"; on its own as a single hold.
        public int? DurationSeconds { get; set; }

        public string? Notes { get; set; }

        public TrainingPlan TrainingPlan { get; set; } = null!;
    }
}
