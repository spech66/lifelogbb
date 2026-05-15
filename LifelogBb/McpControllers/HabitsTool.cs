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
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateHabit", Title = "Create habit entry"), Description("Create a new habit entry")]
        public async Task<HabitOutput?> Create(HabitInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
