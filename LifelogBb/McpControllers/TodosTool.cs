using LifelogBb.ApiDTOs;
using LifelogBb.ApiDTOs.Todos;
using LifelogBb.ApiServices;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class TodosTool : BaseTool<TodosService, TodoInput, TodoOutput>
    {
        public TodosTool(TodosService service) : base(service)
        {
        }

        [McpServerTool(Name = "GetAllTodos", Title = "Get All Todos", ReadOnly = true, OpenWorld = false), Description("Get all todo data, newest first. Optionally filter by providing a JSON filter expression, sort by a field, and limit how many entries are returned.")]
        public async Task<IEnumerable<TodoOutput>> McpGetAll(
            [Description("Optional JSON filter expression")] string? filter = null,
            [Description("Optional sort field, for example \"DueDate\" ascending or \"DueDate_desc\" descending. Defaults to newest first.")] string? sort = null,
            [Description("Optional maximum number of entries to return. Combine with sort to fetch only the entries you need.")] int? limit = null)
        {
            return await GetAllFiltered(filter, sort, limit);
        }

        [McpServerTool(Name = "CreateTodo", Title = "Create todo entry", Destructive = false, OpenWorld = false), Description("Create a new todo entry")]
        public async Task<TodoOutput?> Create(TodoInput model)
        {
            var result = await _service.Create(model);
            return result;
        }

        [McpServerTool(Name = "UpdateTodo", Title = "Update todo entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Update an existing todo entry. All fields of the entry are replaced by the provided values.")]
        public async Task<TodoOutput?> Update([Description("Id of the todo entry to update")] long id, TodoInput model)
        {
            var result = await _service.Update(id, model);
            return result;
        }

        [McpServerTool(Name = "DeleteTodo", Title = "Delete todo entry", Destructive = true, Idempotent = true, OpenWorld = false), Description("Delete an existing todo entry")]
        public async Task<DeleteOutput?> Delete([Description("Id of the todo entry to delete")] long id)
        {
            var deletedId = await _service.Delete(id);
            return deletedId == null ? null : new DeleteOutput() { Id = deletedId.Value };
        }
    }
}
