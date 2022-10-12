using Microsoft.AspNetCore.Mvc;
using LifelogBb.ApiServices;
using LifelogBb.ApiDTOs.Weights;

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
