using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public enum BucketListStatus
    {
        Wish,
        InProgress,
        Reached
    }

    public class BucketList : BaseEntity
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public BucketListStatus Status { get; set; }

        public string? ImageName { get; set; }

        public byte[]? ImageData { get; set; }
    }
}
