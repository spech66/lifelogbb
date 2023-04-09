using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
