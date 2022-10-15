using AutoMapper;
using LifelogBb.Models.Entities;
using LifelogBb.Models.Weights;
using LifelogBb.Models.Journals;
using LifelogBb.Models.StrengthTrainings;
using LifelogBb.Models.EnduranceTrainings;

namespace LifelogBb.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Weights
            CreateMap<EditWeightViewModel, Weight>();
            CreateMap<Weight, EditWeightViewModel>();

            // Journals
            CreateMap<EditJournalViewModel, Journal>();
            CreateMap<Journal, EditJournalViewModel>();

            // StrengthTrainings
            CreateMap<EditStrengthTrainingViewModel, StrengthTraining>();
            CreateMap<StrengthTraining, EditStrengthTrainingViewModel>();

            // EnduranceTrainings
            CreateMap<EditEnduranceTrainingViewModel, EnduranceTraining>();
            CreateMap<EnduranceTraining, EditEnduranceTrainingViewModel>();
        }
    }
}
