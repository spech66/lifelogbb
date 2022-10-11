using Microsoft.AspNetCore.Mvc;
using LifelogBb.ApiServices;
using LifelogBb.DTOs.Weights;

namespace LifelogBb.ApiControllers
{
    [Route("api/weights")]
    public class WeightsApiController : BaseCRUDController<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsApiController(WeightsService service) : base(service)
        {
        }
    }
}
