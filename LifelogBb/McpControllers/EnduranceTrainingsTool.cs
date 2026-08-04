using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class EnduranceTrainingsTool : BaseTool<EnduranceTrainingsService, EnduranceTrainingInput, EnduranceTrainingOutput>
    {
        public EnduranceTrainingsTool(EnduranceTrainingsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllEnduranceTrainings", Title = "Get All Endurance Trainings", ReadOnly = true, OpenWorld = false), Description("Get all endurance training data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned. For the last workout use limit 1.")]
        public async Task<IEnumerable<EnduranceTrainingOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateEnduranceTraining", Title = "Create endurance training entry", Destructive = false, OpenWorld = false), Description("Create a new endurance training entry")]
        public async Task<EnduranceTrainingOutput?> Create(EnduranceTrainingInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateEnduranceTraining", Title = "Update endurance training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing endurance training entry. All fields of the entry are replaced by the provided values.")]
        public async Task<EnduranceTrainingOutput?> Update([Description("Id of the endurance training entry to update")] long id, EnduranceTrainingInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteEnduranceTraining", Title = "Delete endurance training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing endurance training entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the endurance training entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
