using LifelogBb.ApiServices;
using LifelogBb.DTOs.Weights;

namespace LifelogBb.ApiControllers
{
    public class WeightsApiController : BaseCRUDController<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsApiController(WeightsService service) : base(service)
        {
        }
    }
}
