using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Home
{
    public class IndexDashboardViewModelActivity
    {
        public string Type { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTime? Date { get; set; }
    }

    public class IndexDashboardViewModel
    {
        public List<Weight>? WeightList { get; set; }

        public StrengthTraining? LastStrengthTraining { get; set; }

        public EnduranceTraining? LastEnduranceTraining { get; set; }

        public BucketList? RandomBucketList { get; set; }

        public Quote? RandomQuote { get; set; }

        public List<Todo>? TodoList { get; set; }

        public List<IndexDashboardViewModelActivity>? Activities { get; set; }
    }
}
