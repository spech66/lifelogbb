using LifelogBb.ApiDTOs.TrainingPlans;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/trainingplans")]
    [ApiController]
    public class TrainingPlansApiController : BaseCRUDController<TrainingPlansService, TrainingPlanInput, TrainingPlanOutput>
    {
        public TrainingPlansApiController(TrainingPlansService service) : base(service)
        {
        }

        public class CopyTrainingPlanRequest
        {
            public DateTime? Date { get; set; }
            public string? Name { get; set; }
        }

        // POST: api/trainingplans/5/copy
        [HttpPost("{id}/copy")]
        public async Task<ActionResult<TrainingPlanOutput>> Copy(long id, [FromBody] CopyTrainingPlanRequest? request)
        {
            var result = await _service.CopyPlan(id, request?.Date, request?.Name);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
