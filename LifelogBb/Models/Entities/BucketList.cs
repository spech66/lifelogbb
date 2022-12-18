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

    // for table splitting https://learn.microsoft.com/en-us/ef/core/modeling/table-splitting
    // because of https://github.com/dotnet/efcore/issues/1387
    [Table("BucketLists")]
    public class BucketList : BaseEntity
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Category { get; set; }

        public BucketListStatus Status { get; set; }

        public string? ImageName { get; set; }

        public BucketListImage? Image { get; set; }

        public BucketList()
        {
            // Default constructor
        }

        public BucketList(string title, string description, string category, BucketListStatus status)
        {
            Title = title;
            Description = description;
            Category = category;
            Status = status;
        }
    }
}
