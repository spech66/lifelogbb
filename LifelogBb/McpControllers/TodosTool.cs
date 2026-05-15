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

        [McpServerTool(Name = "GetAllTodos", Title = "Get All Todos"), Description("Get all todo data. Optionally filter by providing a JSON filter expression.")]
        public async Task<IEnumerable<TodoOutput>> McpGetAll([Description("Optional JSON filter expression")] string? filter = null)
        {
            return await GetAllFiltered(filter);
        }

        [McpServerTool(Name = "CreateTodo", Title = "Create todo entry"), Description("Create a new todo entry")]
        public async Task<TodoOutput?> Create(TodoInput model)
        {
            var result = await _service.Create(model);
            return result;
        }
    }
}
