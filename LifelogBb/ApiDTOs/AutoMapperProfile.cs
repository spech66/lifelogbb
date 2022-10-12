using AutoMapper;
using LifelogBb.DTOs.Weights;
using LifelogBb.Models.Entities;

namespace LifelogBb.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Weights
            CreateMap<WeightInput, Weight>();
            CreateMap<Weight, WeightOutput>();
        }
    }
}
