using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.ApiControllers
{
    [Route("api/weights")]
    [ApiController]
    public class WeightsApiController : BaseCRUDController<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsApiController(WeightsService service) : base(service)
        {
        }
    }
}
