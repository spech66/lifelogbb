using LifelogBb.Models.Entities;

namespace LifelogBb.Models.BucketLists
{
    public class IndexBucketListViewModel
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public BucketListStatus Status { get; set; }

        public string? ImageName { get; set; }
    }
}
