using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    [Table("BucketLists")] // for table splitting https://learn.microsoft.com/en-us/ef/core/modeling/table-splitting
    public class BucketListImage
    {
        [Key, ForeignKey("ImageOf")]
        public long Id { get; set; }

        public byte[]? ImageData { get; set; }

        public BucketList ImageOf { get; set; }
    }
}

