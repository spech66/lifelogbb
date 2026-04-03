using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Goals
{
    public class GoalIndexViewModel
    {
        public int TotalCount { get; set; }

        public int ActiveCount { get; set; }

        public int CompletedCount { get; set; }

        public int OverdueCount { get; set; }

        public List<Goal> ActiveGoals { get; set; } = new();

        public List<Goal> RecentlyCompleted { get; set; } = new();
    }
}
