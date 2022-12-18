using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LifelogBb.Models.Goals
{
    public class EditGoalViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public double? TargetValue { get; set; }

        public double? CurrentValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }
    }
}
