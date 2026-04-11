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

        [McpServerTool(Name= "GetAllWeights", Title = "Get All Weights"), Description("Get all weight data")]
        public override Task<ActionResult<IEnumerable<WeightOutput>>> GetAll()
        {
            return base.GetAll();
        }

        [McpServerTool(Name = "CreateWeight", Title = "Create weight entry")]
        public override Task<ActionResult<WeightOutput>> Create(WeightInput model)
        {
            return base.Create(model);
        }
    }
}
