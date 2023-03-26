using AutoMapper;
using LifelogBb.Models.Entities;
using LifelogBb.Models.Weights;
using LifelogBb.Models.Journals;
using LifelogBb.Models.StrengthTrainings;
using LifelogBb.Models.EnduranceTrainings;
using LifelogBb.Models.BucketLists;
using LifelogBb.Models.Habits;
using LifelogBb.Models.Todos;
using LifelogBb.Models.Quotes;
using LifelogBb.Models.Goals;

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

            // BucketLists
            CreateMap<EditBucketListViewModel, BucketList>();
            CreateMap<BucketList, EditBucketListViewModel>();          
            CreateMap<CreateBucketListViewModel, BucketList>()
                .ForSourceMember(source => source.ImageData, opt => opt.DoNotValidate());
            CreateMap<EditBucketListViewModel, BucketList>()
                .ForSourceMember(source => source.ImageData, opt => opt.DoNotValidate());
            CreateMap<BucketList, EditBucketListViewModel>()
                .ForMember(dest => dest.ImageData, opt => opt.Ignore());

            // Quotes
            CreateMap<EditQuoteViewModel, Quote>();
            CreateMap<Quote, EditQuoteViewModel>();

            // Todos
            CreateMap<EditTodoViewModel, Todo>();
            CreateMap<Todo, EditTodoViewModel>();

            // Habits
            CreateMap<EditHabitViewModel, Habit>();
            CreateMap<Habit, EditHabitViewModel>();

            // Goals
            CreateMap<EditGoalViewModel, Goal>();
            CreateMap<Goal, EditGoalViewModel>();
        }
    }
}
