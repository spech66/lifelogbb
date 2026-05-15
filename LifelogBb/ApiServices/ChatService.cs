using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using LifelogBb.Models;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class ChatMessage
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
        public string? ToolCallId { get; set; }
        public JsonArray? ToolCalls { get; set; }
    }

    public class ChatService
    {
        private readonly LifelogBbContext _context;
        private readonly ChatToolRegistry _toolRegistry;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            LifelogBbContext context,
            ChatToolRegistry toolRegistry,
            IHttpClientFactory httpClientFactory,
            ILogger<ChatService> logger)
        {
            _context = context;
            _toolRegistry = toolRegistry;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Sends a conversation to the configured AI endpoint, handles tool calls in a loop,
        /// and returns the final assistant response text.
        /// </summary>
        public async Task<string> SendAsync(List<ChatMessage> conversation)
        {
            var config = Config.GetConfig(_context);

            if (string.IsNullOrWhiteSpace(config.ChatApiKey))
            {
                return "Chat is not configured. Please set your API key in the Config page.";
            }

            var maxRoundtrips = config.ChatMaxToolRoundtrips;
            var tools = _toolRegistry.GetToolDefinitions();

            // Build the full message list with system prompt
            var messages = new JsonArray();
            messages.Add(new JsonObject
            {
                ["role"] = "system",
                ["content"] = config.ChatSystemPrompt
            });

            foreach (var msg in conversation)
            {
                var msgObj = new JsonObject
                {
                    ["role"] = msg.Role,
                    ["content"] = msg.Content
                };

                if (msg.ToolCallId != null)
                {
                    msgObj["tool_call_id"] = msg.ToolCallId;
                }

                if (msg.ToolCalls != null)
                {
                    msgObj["tool_calls"] = msg.ToolCalls.DeepClone();
                }

                messages.Add(msgObj);
            }

            for (int roundtrip = 0; roundtrip < maxRoundtrips; roundtrip++)
            {
                var response = await CallEndpointAsync(config, messages, tools);

                if (response == null)
                {
                    return "Error: Failed to get a response from the AI endpoint.";
                }

                var choices = response["choices"]?.AsArray();
                if (choices == null || choices.Count == 0)
                {
                    return "Error: No response choices returned from the AI endpoint.";
                }

                var choice = choices[0]?.AsObject();
                var message = choice?["message"]?.AsObject();
                if (message == null)
                {
                    return "Error: Invalid response format from the AI endpoint.";
                }

                var finishReason = choice?["finish_reason"]?.GetValue<string>();
                var toolCalls = message["tool_calls"]?.AsArray();

                // Append assistant message to conversation
                messages.Add(message.DeepClone());

                if (finishReason == "tool_calls" || (toolCalls != null && toolCalls.Count > 0))
                {
                    // Execute each tool call and append results
                    foreach (var toolCall in toolCalls!)
                    {
                        var id = toolCall?["id"]?.GetValue<string>() ?? "";
                        var function = toolCall?["function"]?.AsObject();
                        var functionName = function?["name"]?.GetValue<string>() ?? "";
                        var functionArgs = function?["arguments"]?.GetValue<string>();

                        _logger.LogInformation("Chat tool call: {ToolName} with args: {Args}", functionName, functionArgs);

                        var toolResult = await _toolRegistry.ExecuteToolAsync(functionName, functionArgs);

                        messages.Add(new JsonObject
                        {
                            ["role"] = "tool",
                            ["tool_call_id"] = id,
                            ["content"] = toolResult
                        });
                    }
                }
                else
                {
                    // No tool calls, return the assistant's text
                    return message["content"]?.GetValue<string>() ?? "";
                }
            }

            return "Error: Maximum tool call roundtrips reached. The AI may need a simpler question.";
        }

        private async Task<JsonObject?> CallEndpointAsync(Config config, JsonArray messages, JsonArray tools)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ChatApiKey);

                var requestBody = new JsonObject
                {
                    ["model"] = config.ChatModel,
                    ["messages"] = messages.DeepClone(),
                    ["tools"] = tools.DeepClone()
                };

                var content = new StringContent(
                    requestBody.ToJsonString(),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(config.ChatEndpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("AI endpoint returned {StatusCode}: {Body}", response.StatusCode, responseBody);
                    return null;
                }

                return JsonSerializer.Deserialize<JsonObject>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI endpoint");
                return null;
            }
        }
    }
}
