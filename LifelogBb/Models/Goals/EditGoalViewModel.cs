using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Goals
{
    public class EditGoalViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public double? InitialValue { get; set; }

        public double? TargetValue { get; set; }

        public double? CurrentValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Display(Name = "Reminders")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
