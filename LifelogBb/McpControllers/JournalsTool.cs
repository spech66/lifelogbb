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
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateJournal", Title = "Create journal entry"), Description("Create a new journal entry")]
        public async Task<JournalOutput?> Create(JournalInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
