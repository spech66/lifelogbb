using LifelogBb.ApiControllers;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;
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

        [McpServerTool(Name = "GetAllWeights", Title = "Get All Weights"), Description("Get all weight data")]
        public async Task<IEnumerable<WeightOutput>> McpGetAll()
        {
            var result = await _service.GetAll();
            return result.Value ?? [];
        }

        [McpServerTool(Name = "CreateWeight", Title = "Create weight entry"), Description("Create a new weight entry")]
        public async Task<WeightOutput?> Create(WeightInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
