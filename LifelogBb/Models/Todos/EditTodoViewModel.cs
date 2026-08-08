using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Todos
{
    public class EditTodoViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? DueDate { get; set; }

        [Display(Name = "Reminders")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public DateTime? Completed { get; set; }

        [DefaultValue(false)]
        public bool IsImportant { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
