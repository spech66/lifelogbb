using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class StrengthTrainingsTool : BaseTool<StrengthTrainingsService, StrengthTrainingInput, StrengthTrainingOutput>
    {
        public StrengthTrainingsTool(StrengthTrainingsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllStrengthTrainings", Title = "Get All Strength Trainings", ReadOnly = true, OpenWorld = false), Description("Get all strength training data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned. For the last workout use limit 1.")]
        public async Task<IEnumerable<StrengthTrainingOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateStrengthTraining", Title = "Create strength training entry", Destructive = false, OpenWorld = false), Description("Create a new strength training entry")]
        public async Task<StrengthTrainingOutput?> Create(StrengthTrainingInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateStrengthTraining", Title = "Update strength training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing strength training entry. All fields of the entry are replaced by the provided values.")]
        public async Task<StrengthTrainingOutput?> Update([Description("Id of the strength training entry to update")] long id, StrengthTrainingInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteStrengthTraining", Title = "Delete strength training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing strength training entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the strength training entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
