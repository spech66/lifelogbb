using LifelogBb.ApiServices;
using LifelogBb.DTOs.Weights;

namespace LifelogBb.ApiControllers
{
    public class WeightsController : BaseCRUDController<WeightsService, WeightInput, WeightOutput>
    {
        public WeightsController(WeightsService service) : base(service)
        {
        }
    }
}
