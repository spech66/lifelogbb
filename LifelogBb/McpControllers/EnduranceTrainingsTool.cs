using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class EnduranceTrainingsTool : BaseTool<EnduranceTrainingsService, EnduranceTrainingInput, EnduranceTrainingOutput>
    {
        public EnduranceTrainingsTool(EnduranceTrainingsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllEnduranceTrainings", Title = "Get All Endurance Trainings"), Description("Get all endurance training data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<EnduranceTrainingOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
