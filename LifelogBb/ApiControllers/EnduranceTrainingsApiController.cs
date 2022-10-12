using LifelogBb.ApiServices;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/endurancetrainings")]
    [ApiController]
    public class EnduranceTrainingsApiController : BaseCRUDController<EnduranceTrainingsService, EnduranceTrainingInput, EnduranceTrainingOutput>
    {
        public EnduranceTrainingsApiController(EnduranceTrainingsService service) : base(service)
        {
        }
    }
}
