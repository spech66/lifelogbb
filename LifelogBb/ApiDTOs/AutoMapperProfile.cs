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
