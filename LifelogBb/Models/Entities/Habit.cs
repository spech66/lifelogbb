using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
