using LifelogBb.Models.Entities;

namespace LifelogBb.Models.TrainingPlans
{
    public class TrainingPlanIndexViewModel
    {
        public List<TrainingPlan> Templates { get; set; } = new();
        public List<TrainingPlan> DayPlans { get; set; } = new();
        public List<TrainingPlan> Archived { get; set; } = new();
    }
}
