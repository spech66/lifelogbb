using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class ChatSession : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = "New Chat";

        public ICollection<ChatSessionMessage> Messages { get; set; } = new List<ChatSessionMessage>();
    }
}
