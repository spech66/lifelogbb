using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.ApiDTOs.Goals
{
    public class GoalInput
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public double? InitialValue { get; set; }

        public double? TargetValue { get; set; }

        public double? CurrentValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Description("Reminders before the end date as a comma separated list of negative ISO-8601 durations, for example \"-PT15M,-P1D\"")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
