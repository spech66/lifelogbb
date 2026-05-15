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

        [McpServerTool(Name = "GetAllWeights", Title = "Get All Weights"), Description("Get all weight data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<WeightOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateWeight", Title = "Create weight entry"), Description("Create a new weight entry")]
        public async Task<WeightOutput?> Create(WeightInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
