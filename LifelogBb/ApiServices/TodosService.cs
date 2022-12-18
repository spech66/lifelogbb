using AutoMapper;
using LifelogBb.ApiDTOs.Todos;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class TodosService : BaseCRUDService<Todo, TodoInput, TodoOutput>
    {
        public TodosService(IRepository<Todo> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
