namespace LifelogBb.Models.Chat
{
    public class ChatViewModel
    {
        public List<ChatMessageViewModel> Messages { get; set; } = new();
        public string UserInput { get; set; } = "";
        public bool IsConfigured { get; set; }
    }
}
