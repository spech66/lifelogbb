using LifelogBb.Interfaces.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Habits
{
    public class HabitOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? Frequency { get; set; } // Interval

        public string? FrequencyUnit { get; set; } // daily, weekly, ...

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ExtraRules { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
