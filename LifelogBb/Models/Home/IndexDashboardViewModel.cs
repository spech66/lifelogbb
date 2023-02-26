using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Home
{
    public class IndexDashboardViewModel
    {
        public Weight? LastWeight { get; set; }

        public StrengthTraining? LastStrengthTraining { get; set; }

        public EnduranceTraining? LastEnduranceTraining { get; set; }

        public BucketList? RandomBucketList { get; set; }

        public Quote? RandomQuote { get; set; }
    }
}
