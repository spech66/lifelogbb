using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.ApiDTOs.Habits
{
    public class HabitInput
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? RecurrenceRules { get; set; }

        [Description("Reminders before the start date as a comma separated list of negative ISO-8601 durations, for example \"-PT15M,-P1D\"")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
