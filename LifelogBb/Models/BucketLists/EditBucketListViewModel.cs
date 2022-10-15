﻿using LifelogBb.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.BucketLists
{
    public class EditBucketListViewModel
    {
        public long Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public BucketListStatus Status { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageData { get; set; }
    }
}
