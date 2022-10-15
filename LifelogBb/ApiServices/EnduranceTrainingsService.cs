using AutoMapper;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiServices
{
    public class EnduranceTrainingsService : BaseCRUDService<EnduranceTraining, EnduranceTrainingInput, EnduranceTrainingOutput>
    {
        public EnduranceTrainingsService(IRepository<EnduranceTraining> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
