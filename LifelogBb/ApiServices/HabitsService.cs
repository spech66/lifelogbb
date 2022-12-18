using AutoMapper;
using LifelogBb.ApiDTOs.Habits;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class HabitsService : BaseCRUDService<Habit, HabitInput, HabitOutput>
    {
        public HabitsService(IRepository<Habit> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
