using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiDTOs.Goals;
using LifelogBb.ApiDTOs.Habits;
using LifelogBb.ApiDTOs.Journals;
using LifelogBb.ApiDTOs.Quotes;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.ApiDTOs.Todos;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using LifelogBb.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiServices
{
    /// <summary>
    /// Defines the available tools for AI chat, mirroring the read-only MCP tools.
    /// Builds OpenAI-compatible tool schemas and dispatches tool calls to the correct service.
    /// </summary>
    public class ChatToolRegistry
    {
        private const int DefaultToolResultLimit = 25;
        private const int MaxToolResultLimit = 50;

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

            tools.Add(CreateToolDefinition("GetAllWeights", "Get recent weight data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllJournals", "Get recent journal data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllTodos", "Get recent todo data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllGoals", "Get recent goal data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllHabits", "Get recent habit data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllQuotes", "Get recent quote data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllStrengthTrainings", "Get recent strength training data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));
            tools.Add(CreateToolDefinition("GetAllEnduranceTrainings", "Get recent endurance training data ordered by most recently updated first. Results default to 25 records and are capped at 50.",
                CreateToolParameters()));

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
                var limit = DefaultToolResultLimit;
                if (!string.IsNullOrEmpty(argumentsJson))
                {
                    var args = JsonSerializer.Deserialize<JsonElement>(argumentsJson);
                    if (args.TryGetProperty("filter", out var filterEl) && filterEl.ValueKind == JsonValueKind.String)
                    {
                        filter = filterEl.GetString();
                    }

                    if (args.TryGetProperty("limit", out var limitEl) &&
                        limitEl.ValueKind == JsonValueKind.Number &&
                        limitEl.TryGetInt32(out var requestedLimit))
                    {
                        limit = NormalizeLimit(requestedLimit);
                    }
                }

                object? result = toolName switch
                {
                    "GetAllWeights" => await GetAllFromRepository<Weight, WeightOutput>(filter, limit),
                    "GetAllJournals" => await GetAllFromRepository<Journal, JournalOutput>(filter, limit),
                    "GetAllTodos" => await GetAllFromRepository<Todo, TodoOutput>(filter, limit),
                    "GetAllGoals" => await GetAllFromRepository<Goal, GoalOutput>(filter, limit),
                    "GetAllHabits" => await GetAllFromRepository<Habit, HabitOutput>(filter, limit),
                    "GetAllQuotes" => await GetAllFromRepository<Quote, QuoteOutput>(filter, limit),
                    "GetAllStrengthTrainings" => await GetAllFromRepository<StrengthTraining, StrengthTrainingOutput>(filter, limit),
                    "GetAllEnduranceTrainings" => await GetAllFromRepository<EnduranceTraining, EnduranceTrainingOutput>(filter, limit),
                    _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        private async Task<object> GetAllFromRepository<TEntity, TOutput>(string? filter, int limit)
            where TEntity : BaseEntity
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            var entities = await repository.Query
                .FilterByGroup<TEntity>(filter, throwOnInvalidFilter: true)
                .OrderByDescending(entity => entity.UpdatedAt)
                .ThenByDescending(entity => entity.CreatedAt)
                // Take one extra record so the response can tell the model when more matching data exists.
                .Take(limit + 1)
                .ToListAsync();

            var truncated = entities.Count > limit;
            var items = truncated
                ? entities.Take(limit).ToList()
                : entities;

            return new
            {
                items = mapper.Map<List<TOutput>>(items),
                limit,
                truncated
            };
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

        private static int NormalizeLimit(int requestedLimit)
        {
            return Math.Clamp(requestedLimit, 1, MaxToolResultLimit);
        }

        private static JsonObject CreateToolParameters()
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
                    },
                    ["limit"] = new JsonObject
                    {
                        ["type"] = "integer",
                        ["description"] = $"Optional maximum number of records to return. Defaults to {DefaultToolResultLimit} and is capped at {MaxToolResultLimit}.",
                        ["minimum"] = 1,
                        ["maximum"] = MaxToolResultLimit
                    }
                },
                ["required"] = new JsonArray()
            };
        }
    }
}
