using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Entities
{
    public class Habit : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? RecurrenceRules { get; set; }

        [Display(Name = "Reminders")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public Habit()
        {
            // Default constructor
        }

        public Habit(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
