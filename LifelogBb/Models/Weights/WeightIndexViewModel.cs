using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Weights
{
    public class WeightIndexViewModel
    {
        public Weight? Latest { get; set; }

        public Weight? Previous { get; set; }

        public double? Change30Days { get; set; }

        public double? AllTimeMin { get; set; }

        public double? AllTimeMax { get; set; }

        public List<Weight> RecentEntries { get; set; } = new();

        public Config Config { get; set; } = new();
    }
}
