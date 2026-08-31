using LifelogBb.ApiDTOs;
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

        [McpServerTool(Name = "GetAllJournals", Title = "Get All Journals", ReadOnly = true, OpenWorld = false), Description("Get all journal data, newest first by journal date. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned. For the most recent entries use a small limit.")]
        public async Task<IEnumerable<JournalOutput>> McpGetAll(
            [Description("Optional JSON filter expression, passed as a string containing a filter group: {\"operator\":\"And\",\"conditions\":[{\"field\":\"FieldName\",\"operator\":\"Equal\",\"value\":\"someValue\"}]}. The group operator is And or Or, conditions support Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, Contains, NotContains, In and NotIn, and value is always a string (In/NotIn take a comma-separated list). Groups can be nested via \"groups\".")] string? filter = null,
            [Description("Optional sort field, for example \"Date\" ascending or \"Date_desc\" descending. Date is the day the entry is about, CreatedAt is when it was written. Defaults to the newest journal date first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateJournal", Title = "Create journal entry", Destructive = false, OpenWorld = false), Description("Create a new journal entry")]
        public async Task<JournalOutput?> Create(JournalInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateJournal", Title = "Update journal entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing journal entry. All fields of the entry are replaced by the provided values.")]
        public async Task<JournalOutput?> Update([Description("Id of the journal entry to update")] long id, JournalInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteJournal", Title = "Delete journal entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing journal entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the journal entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
