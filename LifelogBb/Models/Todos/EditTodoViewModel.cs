using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LifelogBb.Models.Todos
{
    public class EditTodoViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsImportant { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
