using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.Goals;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class GoalsTool : BaseTool<GoalsService, GoalInput, GoalOutput>
    {
        public GoalsTool(GoalsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllGoals", Title = "Get All Goals", ReadOnly = true, OpenWorld = false), Description("Get all goal data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned.")]
        public async Task<IEnumerable<GoalOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"EndDate\" ascending or \"EndDate_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateGoal", Title = "Create goal entry", Destructive = false, OpenWorld = false), Description("Create a new goal entry")]
        public async Task<GoalOutput?> Create(GoalInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateGoal", Title = "Update goal entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing goal entry. All fields of the entry are replaced by the provided values.")]
        public async Task<GoalOutput?> Update([Description("Id of the goal entry to update")] long id, GoalInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteGoal", Title = "Delete goal entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing goal entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the goal entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
