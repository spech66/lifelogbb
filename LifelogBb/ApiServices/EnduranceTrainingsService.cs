using AutoMapper;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class EnduranceTrainingsService : BaseCRUDService<EnduranceTraining, EnduranceTrainingInput, EnduranceTrainingOutput>
    {
        public EnduranceTrainingsService(IRepository<EnduranceTraining> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
