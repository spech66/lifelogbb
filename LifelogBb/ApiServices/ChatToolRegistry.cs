using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiServices
{
    /// <summary>
    /// Defines the available tools for AI chat, mirroring the read-only MCP tools.
    /// Builds OpenAI-compatible tool schemas and dispatches tool calls to the correct service.
    /// </summary>
    public class ChatToolRegistry
    {
        private readonly IServiceProvider _serviceProvider;

        public ChatToolRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns the OpenAI-compatible tools array for the chat completions request.
        /// Only read tools (GetAll*) are exposed.
        /// </summary>
        public JsonArray GetToolDefinitions()
        {
            var tools = new JsonArray();

            tools.Add(CreateToolDefinition("GetAllWeights", "Get all weight data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllJournals", "Get all journal data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllTodos", "Get all todo data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllGoals", "Get all goal data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllHabits", "Get all habit data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllQuotes", "Get all quote data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllStrengthTrainings", "Get all strength training data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));
            tools.Add(CreateToolDefinition("GetAllEnduranceTrainings", "Get all endurance training data. Optionally filter by providing a JSON filter expression.",
                CreateFilterParameter()));

            return tools;
        }

        /// <summary>
        /// Dispatches a tool call by name with the given arguments JSON, returning the result as a string.
        /// </summary>
        public async Task<string> ExecuteToolAsync(string toolName, string? argumentsJson)
        {
            try
            {
                string? filter = null;
                if (!string.IsNullOrEmpty(argumentsJson))
                {
                    var args = JsonSerializer.Deserialize<JsonElement>(argumentsJson);
                    if (args.TryGetProperty("filter", out var filterEl) && filterEl.ValueKind == JsonValueKind.String)
                    {
                        filter = filterEl.GetString();
                    }
                }

                object? result = toolName switch
                {
                    "GetAllWeights" => await GetAllFromService<WeightsService>(filter),
                    "GetAllJournals" => await GetAllFromService<JournalsService>(filter),
                    "GetAllTodos" => await GetAllFromService<TodosService>(filter),
                    "GetAllGoals" => await GetAllFromService<GoalsService>(filter),
                    "GetAllHabits" => await GetAllFromService<HabitsService>(filter),
                    "GetAllQuotes" => await GetAllFromService<QuotesService>(filter),
                    "GetAllStrengthTrainings" => await GetAllFromService<StrengthTrainingsService>(filter),
                    "GetAllEnduranceTrainings" => await GetAllFromService<EnduranceTrainingsService>(filter),
                    _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        private async Task<object?> GetAllFromService<TService>(string? filter) where TService : notnull
        {
            var service = _serviceProvider.GetRequiredService<TService>();

            // All services implement IBaseCRUDService<INP, OUTP> which has GetAll() and GetAll(string?)
            // Use dynamic dispatch to call the correct GetAll method
            dynamic dynamicService = service;
            dynamic result = filter != null
                ? await dynamicService.GetAll(filter)
                : await dynamicService.GetAll();

            return result.Value;
        }

        private static JsonObject CreateToolDefinition(string name, string description, JsonObject parameters)
        {
            return new JsonObject
            {
                ["type"] = "function",
                ["function"] = new JsonObject
                {
                    ["name"] = name,
                    ["description"] = description,
                    ["parameters"] = parameters
                }
            };
        }

        private static JsonObject CreateFilterParameter()
        {
            return new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["filter"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "Optional JSON filter expression to filter results"
                    }
                },
                ["required"] = new JsonArray()
            };
        }
    }
}
