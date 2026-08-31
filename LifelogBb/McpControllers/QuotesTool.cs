using LifelogBb.ApiDTOs;
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

        [McpServerTool(Name = "GetAllQuotes", Title = "Get All Quotes", ReadOnly = true, OpenWorld = false), Description("Get all quote data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned.")]
        public async Task<IEnumerable<QuoteOutput>> McpGetAll(
            [Description("Optional JSON filter expression, passed as a string containing a filter group: {\"operator\":\"And\",\"conditions\":[{\"field\":\"FieldName\",\"operator\":\"Equal\",\"value\":\"someValue\"}]}. The group operator is And or Or, conditions support Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, Contains, NotContains, In and NotIn, and value is always a string (In/NotIn take a comma-separated list). Groups can be nested via \"groups\".")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateQuote", Title = "Create quote entry", Destructive = false, OpenWorld = false), Description("Create a new quote entry")]
        public async Task<QuoteOutput?> Create(QuoteInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateQuote", Title = "Update quote entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing quote entry. All fields of the entry are replaced by the provided values.")]
        public async Task<QuoteOutput?> Update([Description("Id of the quote entry to update")] long id, QuoteInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteQuote", Title = "Delete quote entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing quote entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the quote entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
