using LifelogBb.Models.Entities;
using LifelogBb.Utilities;

namespace LifelogBb.Models.Home
{
    public class DashboardViewModelActivity
    {
        public EntityType Type { get; set; } = EntityType.Unknown;

        public string Text { get; set; } = string.Empty;

        public DateTime? Date { get; set; }
    }

    public class DashboardViewModel
    {
        public List<Weight>? WeightList { get; set; }

        public StrengthTraining? LastStrengthTraining { get; set; }

        public EnduranceTraining? LastEnduranceTraining { get; set; }

        public BucketList? RandomBucketList { get; set; }

        public Quote? RandomQuote { get; set; }

        public List<Todo>? TodoList { get; set; }

        public List<Goal>? GoalList { get; set; }

        public List<Habit>? HabitList { get; set; }

        public List<DashboardViewModelActivity>? Activities { get; set; }
    }
}
