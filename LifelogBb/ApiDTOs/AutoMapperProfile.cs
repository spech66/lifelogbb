using AutoMapper;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiDTOs.Journals;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiDTOs.StrengthTrainings;
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

            // Journals
            CreateMap<JournalInput, Journal>();
            CreateMap<Journal, JournalOutput>();

            // EnduranceTrainings
            CreateMap<EnduranceTrainingInput, EnduranceTraining>();
            CreateMap<EnduranceTraining, EnduranceTrainingOutput>();

            // StrengthTrainings
            CreateMap<StrengthTrainingInput, StrengthTraining>();
            CreateMap<StrengthTraining, StrengthTrainingOutput>();
        }
    }
}
