using LifelogBb.ApiDTOs.Todos;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/todos")]
    [ApiController]
    public class TodosApiController : BaseCRUDController<TodosService, TodoInput, TodoOutput>
    {
        public TodosApiController(TodosService service) : base(service)
        {
        }
    }
}
