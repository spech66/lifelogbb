using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LifelogBb.Models.Habits
{
    public class EditHabitViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? Frequency { get; set; } // Interval

        public string? FrequencyUnit { get; set; } // daily, weekly, ...

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ExtraRules { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }
    }
}
