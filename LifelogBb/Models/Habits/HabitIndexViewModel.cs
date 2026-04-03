using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Habits
{
    public class HabitOccurrenceViewModel
    {
        public long HabitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class HabitIndexViewModel
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public int TodayCount { get; set; }
        public List<Habit> ActiveHabits { get; set; } = new();
        public List<Habit> RecentlyCompleted { get; set; } = new();
        public List<HabitOccurrenceViewModel> UpcomingOccurrences { get; set; } = new();
    }
}
