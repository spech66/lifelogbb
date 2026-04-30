using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.ApiControllers
{
    [Route("api/weights")]
    [ApiController]
    [McpServerToolType]
    public class WeightsApiController : BaseCRUDController<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsApiController(WeightsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllWeights", Title = "Get All Weights"), Description("Get all weight data")]
        public new async Task<IEnumerable<WeightOutput>> GetAll()
        {
            var result = await _service.GetAll();
            return result.Value ?? [];
        }

        [McpServerTool(Name = "CreateWeight", Title = "Create weight entry"), Description("Create a new weight entry")]
        public new async Task<WeightOutput?> Create(WeightInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
