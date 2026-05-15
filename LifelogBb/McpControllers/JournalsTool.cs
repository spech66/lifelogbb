using LifelogBb.ApiDTOs.Journals;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class JournalsTool : BaseTool<JournalsService, JournalInput, JournalOutput>
    {
        public JournalsTool(JournalsService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllJournals", Title = "Get All Journals"), Description("Get all journal data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<JournalOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
