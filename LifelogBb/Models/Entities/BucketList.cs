using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public enum BucketListStatus
    {
        Wish,
        InProgress,
        Reached
    }

    [Table("BucketLists")] // for table splitting https://learn.microsoft.com/en-us/ef/core/modeling/table-splitting
    public class BucketList : BaseEntity
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public BucketListStatus Status { get; set; }

        public string? ImageName { get; set; }

        public BucketListImage? Image { get; set; }
    }
}
