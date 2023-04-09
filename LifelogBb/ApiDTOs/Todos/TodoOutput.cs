using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LifelogBb.ApiDTOs.Todos
{
    public class TodoOutput : IBaseOutput
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int Progress { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? Completed { get; set; }

        public bool IsImportant { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
