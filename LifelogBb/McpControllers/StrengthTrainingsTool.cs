using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class StrengthTrainingsTool : BaseTool<StrengthTrainingsService, StrengthTrainingInput, StrengthTrainingOutput>
    {
        public StrengthTrainingsTool(StrengthTrainingsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllStrengthTrainings", Title = "Get All Strength Trainings"), Description("Get all strength training data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<StrengthTrainingOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
