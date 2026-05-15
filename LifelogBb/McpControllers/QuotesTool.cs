using LifelogBb.ApiDTOs.Quotes;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class QuotesTool : BaseTool<QuotesService, QuoteInput, QuoteOutput>
    {
        public QuotesTool(QuotesService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllQuotes", Title = "Get All Quotes"), Description("Get all quote data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<QuoteOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateQuote", Title = "Create quote entry"), Description("Create a new quote entry")]
        public async Task<QuoteOutput?> Create(QuoteInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
