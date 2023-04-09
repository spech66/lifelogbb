﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LifelogBb.ApiDTOs.Todos
{
    public class TodoInput
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public DateTime? Completed { get; set; }

        [DefaultValue(false)]
        public bool IsImportant { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
