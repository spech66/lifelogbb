using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiControllers
{
    [Route("api/strengthtrainings")]
    [ApiController]
    public class StrengthTrainingsApiController : BaseCRUDController<StrengthTrainingsService, StrengthTrainingInput, StrengthTrainingOutput>
    {
        public StrengthTrainingsApiController(StrengthTrainingsService service) : base(service)
        {
        }
    }
}
