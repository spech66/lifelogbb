using LifelogBb.ApiDTOs.Habits;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class HabitsTool : BaseTool<HabitsService, HabitInput, HabitOutput>
    {
        public HabitsTool(HabitsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllHabits", Title = "Get All Habits"), Description("Get all habit data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<HabitOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
