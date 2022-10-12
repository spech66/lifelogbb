using AutoMapper;
using LifelogBb.DTOs.Weights;
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
