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

        [McpServerTool(Name = "GetAllGoals", Title = "Get All Goals"), Description("Get all goal data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<GoalOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
