using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.TrainingPlans;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class TrainingPlansTool : BaseTool<TrainingPlansService, TrainingPlanInput, TrainingPlanOutput>
    {
        public TrainingPlansTool(TrainingPlansService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllTrainingPlans", Title = "Get All Training Plans", ReadOnly = true, OpenWorld = false), Description("Get strength training plans, newest first, including their planned sets in order. A plan with no Date is a reusable base/template plan; a plan with a Date is a concrete plan for that specific day. Optionally filter (e.g. {\"IsArchived\":false}), sort, and limit results.")]
        public async Task<IEnumerable<TrainingPlanOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"CreatedAt\" ascending or \"CreatedAt_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateTrainingPlan", Title = "Create training plan", Destructive = false, OpenWorld = false), Description("Create a new strength training plan with its planned sets in one call. The order of the Sets array becomes the order the sets are performed in. Leave Date empty for a base/template plan, or set it to create a concrete day plan.")]
        public async Task<TrainingPlanOutput?> Create(TrainingPlanInput model)
        {
            return await _service.Create(model);
        }

        [McpServerTool(Name = "UpdateTrainingPlan", Title = "Update training plan", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing training plan. All fields are replaced by the provided values, including the full list of Sets: sets not included are removed. Logged strength trainings that reference a removed set keep their history but lose that link. Prefer editing templates and using CopyTrainingPlan for individual days rather than rewriting a day plan that already has logged trainings.")]
        public async Task<TrainingPlanOutput?> Update([Description("Id of the training plan to update")] long id, TrainingPlanInput model)
        {
            return await _service.Update(id, model);
        }

        [McpServerTool(Name = "DeleteTrainingPlan", Title = "Delete training plan", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete a training plan and its planned sets. Strength trainings already logged against it are kept, only the link to the plan is cleared.")]
        public async Task<DeleteOutput?> Delete([Description("Id of the training plan to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }

        [McpServerTool(Name = "CopyTrainingPlan", Title = "Copy training plan", Destructive = false, OpenWorld = false), Description("Duplicate an existing training plan together with all its planned sets. Useful for creating a new version of a base plan, or for deriving a concrete day plan (pass a date) from a template or from a previous day's plan.")]
        public async Task<TrainingPlanOutput?> Copy(
            [Description("Id of the plan to copy")] long sourceId,
            [Description("Optional date (yyyy-MM-dd) to make the copy a concrete day plan instead of another template")] DateTime? date = null,
            [Description("Optional name for the copy. Defaults to \"<source name> (Copy)\"")] string? name = null)
        {
            return await _service.CopyPlan(sourceId, date, name);
        }
    }
}
