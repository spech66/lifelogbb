using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Entities
{
    public class Todo : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Start")]
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Due")]
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Reminders")]
        [RegularExpression(AlarmHelper.Pattern, ErrorMessage = "Reminders must be a comma separated list of negative ISO-8601 durations like -PT15M,-P1D")]
        public string? Alarms { get; set; }

        [DisplayName("Progress")]
        [Range(0, 100)]
        public int Progress { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        [Display(Name = "Completed")]
        [DataType(DataType.DateTime)]
        public DateTime? Completed { get; set; }

        [DefaultValue(false)]
        public bool IsImportant { get; set; }

        public Todo()
        {
            // Default constructor
        }

        public Todo(string title, string description, DateTime dueDate, bool isCompleted, bool isImportant)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            IsCompleted = isCompleted;
            IsImportant = isImportant;
        }
    }
}
