using AutoMapper;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class WeightsService : BaseCRUDService<Weight, WeightInput, WeightOutput>
    {
        public WeightsService(IRepository<Weight> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
