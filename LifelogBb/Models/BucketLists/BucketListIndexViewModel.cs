using LifelogBb.Models.Entities;
using LifelogBb.Utilities;

namespace LifelogBb.Models.BucketLists
{
    public class BucketListIndexViewModel
    {
        public int TotalCount { get; set; }

        public int WishCount { get; set; }

        public int InProgressCount { get; set; }

        public int ReachedCount { get; set; }

        public required PaginatedList<BucketList> List { get; set; }
    }
}
