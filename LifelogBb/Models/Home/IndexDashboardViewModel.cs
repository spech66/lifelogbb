using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Home
{
    public class IndexDashboardViewModel
    {
        public DateTime? LastStrengthTraining { get; set; }

        public DateTime? LastEnduranceTraining { get; set; }

        public BucketList? RandomBucketList { get; set; }

        public Quote? RandomQuote { get; set; }
    }
}
