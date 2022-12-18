using LifelogBb.Interfaces.DTOs;
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

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsImportant { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
