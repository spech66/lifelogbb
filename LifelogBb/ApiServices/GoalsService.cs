using AutoMapper;
using LifelogBb.ApiDTOs.Goals;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class GoalsService : BaseCRUDService<Goal, GoalInput, GoalOutput>
    {
        public GoalsService(IRepository<Goal> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
