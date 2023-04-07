using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Home
{
    public class EditConfigViewModel
    {
        public long Id { get; set; } // For model/view generation only

        public DayOfWeek StartOfWeek { get; set; }

        public Measurements UnitsType { get; set; }

        public int BucketListPageSize { get; set; }

        /*public int EnduranceTrainingPageSize { get; set; }

        public int GoalPageSize { get; set; }

        public int HabitPageSize { get; set; }

        public int JournalPageSize { get; set; }

        public int QuotePageSize { get; set; }

        public int StrengthTrainingPageSize { get; set; }

        public int TodoPageSize { get; set; }*/

        public int WeightPageSize { get; set; }
    }
}
