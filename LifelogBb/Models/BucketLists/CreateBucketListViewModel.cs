using LifelogBb.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.BucketLists
{
    public class CreateBucketListViewModel
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public BucketListStatus Status { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageData { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
