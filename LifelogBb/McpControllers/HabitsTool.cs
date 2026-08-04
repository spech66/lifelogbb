using LifelogBb.ApiDTOs;
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

        [McpServerTool(Name = "GetAllHabits", Title = "Get All Habits", ReadOnly = true, OpenWorld = false), Description("Get all habit data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned.")]
        public async Task<IEnumerable<HabitOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateHabit", Title = "Create habit entry", Destructive = false, OpenWorld = false), Description("Create a new habit entry")]
        public async Task<HabitOutput?> Create(HabitInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateHabit", Title = "Update habit entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing habit entry. All fields of the entry are replaced by the provided values.")]
        public async Task<HabitOutput?> Update([Description("Id of the habit entry to update")] long id, HabitInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteHabit", Title = "Delete habit entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing habit entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the habit entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
