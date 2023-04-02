using Microsoft.VisualBasic;
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

    public class BucketList : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public BucketListStatus Status { get; set; }

        public string? ImageName { get; set; }

        public string? ImageFileName { get; set; }

        public BucketList()
        {
            // Default constructor
        }

        public BucketList(string title, string description, BucketListStatus status)
        {
            Title = title;
            Description = description;
            Status = status;
        }
    }
}
