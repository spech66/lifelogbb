using LifelogBb.ApiServices;
using LifelogBb.Models;
using LifelogBb.Models.Chat;
using LifelogBb.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Westwind.AspNetCore.Markdown;

namespace LifelogBb.Controllers
{
    public class ChatController : Controller
    {
        private readonly LifelogBbContext _context;
        private readonly ChatService _chatService;

        public ChatController(LifelogBbContext context, ChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            var config = Config.GetConfig(_context);
            var model = new ChatViewModel
            {
                IsConfigured = !string.IsNullOrWhiteSpace(config.ChatApiKey)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([FromBody] ChatSendRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Json(new { error = "Message is required." });
            }

            var conversation = new List<ChatMessage>();

            // Add existing conversation history
            if (request.History != null)
            {
                foreach (var msg in request.History)
                {
                    conversation.Add(new ChatMessage
                    {
                        Role = msg.Role,
                        Content = msg.Content
                    });
                }
            }

            // Add the new user message
            conversation.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Message
            });

            var response = await _chatService.SendAsync(conversation);
            var html = Markdown.Parse(response);

            return Json(new { response = html });
        }
    }

    public class ChatSendRequest
    {
        public string Message { get; set; } = "";
        public List<ChatMessageViewModel>? History { get; set; }
    }
}
