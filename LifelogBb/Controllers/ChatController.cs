using System.Collections.Concurrent;
using LifelogBb.ApiServices;
using LifelogBb.Models;
using LifelogBb.Models.Chat;
using LifelogBb.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Westwind.AspNetCore.Markdown;

namespace LifelogBb.Controllers
{
    public partial class ChatController : Controller
    {
        private const int DefaultSessionNameMaxLength = 50;
        private const int MaxConversationMessages = 20;
        private static readonly ConcurrentDictionary<long, SemaphoreSlim> SessionMessageLocks = new();
        private readonly LifelogBbContext _context;
        private readonly ChatService _chatService;

        public ChatController(LifelogBbContext context, ChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public async Task<IActionResult> Index(long? id)
        {
            var config = Config.GetConfig(_context);
            var sessions = await _context.ChatSessions
                .OrderByDescending(s => s.UpdatedAt)
                .Select(s => new ChatSessionListItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            ChatSession? activeSession = null;
            if (id.HasValue)
            {
                activeSession = await _context.ChatSessions
                    .Include(s => s.Messages.OrderBy(m => m.SortOrder))
                    .FirstOrDefaultAsync(s => s.Id == id.Value);
            }

            if (!id.HasValue && activeSession == null && sessions.Count > 0)
            {
                activeSession = await _context.ChatSessions
                    .Include(s => s.Messages.OrderBy(m => m.SortOrder))
                    .OrderByDescending(s => s.UpdatedAt)
                    .FirstOrDefaultAsync();
            }

            var messages = new List<ChatMessageViewModel>();
            if (activeSession != null)
            {
                foreach (var msg in activeSession.Messages)
                {
                    messages.Add(new ChatMessageViewModel
                    {
                        Role = msg.Role,
                        Content = msg.Role == "assistant" ? Markdown.Parse(msg.Content) : msg.Content
                    });
                }
            }

            var model = new ChatViewModel
            {
                IsConfigured = !string.IsNullOrWhiteSpace(config.ChatApiKey),
                Sessions = sessions,
                ActiveSessionId = activeSession?.Id,
                ActiveSessionName = activeSession?.Name,
                Messages = messages
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

            // Load existing messages from the session if we have one
            ChatSession? session = null;
            if (request.SessionId.HasValue)
            {
                session = await _context.ChatSessions
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId.Value);
            }

            if (request.SessionId.HasValue && session == null)
            {
                return Json(new { error = "Session not found." });
            }

            if (session != null)
            {
                var historyLimit = MaxConversationMessages - 1;

                // Build conversation from persisted messages, reserving one slot for the current user message
                var historyMessages = await _context.ChatSessionMessages
                    .Where(m => m.ChatSessionId == session.Id)
                    .OrderByDescending(m => m.SortOrder)
                    .Take(historyLimit)
                    .OrderBy(m => m.SortOrder)
                    .ToListAsync();

                foreach (var msg in historyMessages)
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

            // Persist to database
            if (session == null)
            {
                session = new ChatSession
                {
                    Name = request.Message.Length > DefaultSessionNameMaxLength
                        ? request.Message[..DefaultSessionNameMaxLength] + "..."
                        : request.Message
                };
                session.SetCreateFields();
                _context.ChatSessions.Add(session);
                await _context.SaveChangesAsync();
            }

            var sessionMessageLock = SessionMessageLocks.GetOrAdd(session.Id, _ => new SemaphoreSlim(1, 1));
            await sessionMessageLock.WaitAsync();
            try
            {
                var maxSortOrder = await _context.ChatSessionMessages
                    .Where(m => m.ChatSessionId == session.Id)
                    .MaxAsync(m => (int?)m.SortOrder);
                var nextSortOrder = (maxSortOrder ?? -1) + 1;

                var userMsg = new ChatSessionMessage
                {
                    ChatSessionId = session.Id,
                    Role = "user",
                    Content = request.Message,
                    SortOrder = nextSortOrder
                };
                userMsg.SetCreateFields();

                var assistantMsg = new ChatSessionMessage
                {
                    ChatSessionId = session.Id,
                    Role = "assistant",
                    Content = response,
                    SortOrder = nextSortOrder + 1
                };
                assistantMsg.SetCreateFields();

                _context.ChatSessionMessages.AddRange(userMsg, assistantMsg);
                session.SetUpdateFields();
                await _context.SaveChangesAsync();
            }
            finally
            {
                sessionMessageLock.Release();
            }

            return Json(new { response = html, sessionId = session.Id, sessionName = session.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSession()
        {
            var session = new ChatSession
            {
                Name = "New Chat"
            };
            session.SetCreateFields();
            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = session.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenameSession([FromBody] RenameSessionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                return Json(new { error = "Name is required." });
            }

            var session = await _context.ChatSessions.FindAsync(request.Id);
            if (session == null)
            {
                return NotFound();
            }

            var maxLength = typeof(ChatSession)
                .GetProperty(nameof(ChatSession.Name))!
                .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute), false)
                .Cast<System.ComponentModel.DataAnnotations.StringLengthAttribute>()
                .FirstOrDefault()?.MaximumLength ?? 200;
            session.Name = request.Name.Length > maxLength ? request.Name[..maxLength] : request.Name;
            session.SetUpdateFields();
            await _context.SaveChangesAsync();

            return Json(new { success = true, name = session.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSession(long id)
        {
            var session = await _context.ChatSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.ChatSessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

    public class ChatSendRequest
    {
        public string Message { get; set; } = "";
        public long? SessionId { get; set; }
    }

    public class RenameSessionRequest
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
    }
}
