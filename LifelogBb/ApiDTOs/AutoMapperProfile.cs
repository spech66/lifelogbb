using AutoMapper;
using LifelogBb.ApiDTOs.Weights;
using LifelogBb.ApiDTOs.Journals;
using LifelogBb.ApiDTOs.EnduranceTrainings;
using LifelogBb.ApiDTOs.StrengthTrainings;
using LifelogBb.Models.Entities;
using LifelogBb.ApiDTOs.Quotes;
using LifelogBb.ApiDTOs.Todos;
using LifelogBb.ApiDTOs.Habits;
using LifelogBb.ApiDTOs.Goals;
using LifelogBb.ApiDTOs.TrainingPlans;

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
            CreateMap<JournalInput, Journal>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date));
            CreateMap<Journal, JournalOutput>();

            // EnduranceTrainings
            CreateMap<EnduranceTrainingInput, EnduranceTraining>();
            CreateMap<EnduranceTraining, EnduranceTrainingOutput>();

            // StrengthTrainings
            CreateMap<StrengthTrainingInput, StrengthTraining>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => (src.Date ?? DateTime.UtcNow).Date));
            CreateMap<StrengthTraining, StrengthTrainingOutput>();

            // TrainingPlans - Sets are built/replaced explicitly by TrainingPlansService, not by AutoMapper.
            CreateMap<TrainingPlanInput, TrainingPlan>()
                .ForMember(dest => dest.Sets, opt => opt.Ignore());
            CreateMap<TrainingPlanSetInput, TrainingPlanSet>();
            CreateMap<TrainingPlan, TrainingPlanOutput>()
                .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.Sets.OrderBy(s => s.SortOrder)));
            CreateMap<TrainingPlanSet, TrainingPlanSetOutput>();

            // Quotes
            CreateMap<QuoteInput, Quote>();
            CreateMap<Quote, QuoteOutput>();

            // Todos
            CreateMap<TodoInput, Todo>();
            CreateMap<Todo, TodoOutput>();

            // Habits
            CreateMap<HabitInput, Habit>();
            CreateMap<Habit, HabitOutput>();

            // Goals
            CreateMap<GoalInput, Goal>();
            CreateMap<Goal, GoalOutput>();
        }
    }
}
