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
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateEnduranceTraining", Title = "Create endurance training entry"), Description("Create a new endurance training entry")]
        public async Task<EnduranceTrainingOutput?> Create(EnduranceTrainingInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
