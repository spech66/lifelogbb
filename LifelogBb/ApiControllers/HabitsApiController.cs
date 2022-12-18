using LifelogBb.ApiDTOs.Habits;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/habits")]
    [ApiController]
    public class HabitsApiController : BaseCRUDController<HabitsService, HabitInput, HabitOutput>
    {
        public HabitsApiController(HabitsService service) : base(service)
        {
        }
    }
}
