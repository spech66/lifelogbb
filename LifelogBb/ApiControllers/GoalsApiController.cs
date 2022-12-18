using LifelogBb.ApiDTOs.Goals;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/goals")]
    [ApiController]
    public class GoalsApiController : BaseCRUDController<GoalsService, GoalInput, GoalOutput>
    {
        public GoalsApiController(GoalsService service) : base(service)
        {
        }
    }
}
