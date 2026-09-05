using LifelogBb.ApiDTOs;
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

        [McpServerTool(Name = "GetAllStrengthTrainings", Title = "Get All Strength Trainings", ReadOnly = true, OpenWorld = false), Description("Get all strength training entries, newest first: latest training day first, and within a day the set logged last comes first. One entry is one set, so limit 1 returns the single most recent set, not a whole workout -- for the last workout use a larger limit or filter by its date.")]
        public async Task<IEnumerable<StrengthTrainingOutput>> McpGetAll(
            [Description("Optional JSON filter expression, passed as a string containing a filter group: {\"operator\":\"And\",\"conditions\":[{\"field\":\"FieldName\",\"operator\":\"Equal\",\"value\":\"someValue\"}]}. The group operator is And or Or, conditions support Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, Contains, NotContains, In and NotIn, and value is always a string (In/NotIn take a comma-separated list). Groups can be nested via \"groups\".")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateStrengthTraining", Title = "Create strength training entry", Destructive = false, OpenWorld = false), Description("Create a new strength training entry. One entry is one set. Leave Weight empty for bodyweight, band or mobility work -- that is distinct from an explicit 0 and keeps the set out of volume statistics -- and use DurationSeconds for holds such as planks or stretches.")]
        public async Task<StrengthTrainingOutput?> Create(StrengthTrainingInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateStrengthTraining", Title = "Update strength training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing strength training entry. All fields of the entry are replaced by the provided values.")]
        public async Task<StrengthTrainingOutput?> Update([Description("Id of the strength training entry to update")] long id, StrengthTrainingInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteStrengthTraining", Title = "Delete strength training entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing strength training entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the strength training entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
