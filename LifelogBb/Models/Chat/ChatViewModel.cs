namespace LifelogBb.Models.Chat
{
    public class ChatViewModel
    {
        public List<ChatMessageViewModel> Messages { get; set; } = new();
        public string UserInput { get; set; } = "";
        public bool IsConfigured { get; set; }
        public List<ChatSessionListItem> Sessions { get; set; } = new();
        public long? ActiveSessionId { get; set; }
        public string? ActiveSessionName { get; set; }
    }
}
