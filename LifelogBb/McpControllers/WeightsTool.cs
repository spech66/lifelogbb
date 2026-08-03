using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class WeightsTool : BaseTool<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsTool(WeightsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllWeights", Title = "Get All Weights", ReadOnly = true, OpenWorld = false), Description("Get all weight data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned. For the current weight use limit 1.")]
        public async Task<IEnumerable<WeightOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateWeight", Title = "Create weight entry", Destructive = false, OpenWorld = false), Description("Create a new weight entry")]
        public async Task<WeightOutput?> Create(WeightInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateWeight", Title = "Update weight entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing weight entry. All fields of the entry are replaced by the provided values.")]
        public async Task<WeightOutput?> Update([Description("Id of the weight entry to update")] long id, WeightInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteWeight", Title = "Delete weight entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing weight entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the weight entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
