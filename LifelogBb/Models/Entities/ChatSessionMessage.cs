using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class ChatSessionMessage : BaseEntity
    {
        [Required]
        public long ChatSessionId { get; set; }

        [Required]
        public string Role { get; set; } = "user";

        [Required]
        public string Content { get; set; } = "";

        public int SortOrder { get; set; }

        public ChatSession ChatSession { get; set; } = null!;
    }
}
