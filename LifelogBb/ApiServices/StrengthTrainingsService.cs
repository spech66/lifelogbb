using AutoMapper;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class StrengthTrainingsService : BaseCRUDService<StrengthTraining, StrengthTrainingInput, StrengthTrainingOutput>
    {
        public StrengthTrainingsService(IRepository<StrengthTraining> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
