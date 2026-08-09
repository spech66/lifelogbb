using LifelogBb.Models.Entities;

namespace LifelogBb.Models.TrainingPlans
{
    // View model for the Views/TrainingPlans/_PlanSection.cshtml partial used by Index.cshtml.
    public class PlanSectionModel
    {
        public string? Title { get; }
        public string? Subtitle { get; }
        public List<TrainingPlan> Plans { get; }
        public bool ShowStart { get; }

        public PlanSectionModel(string? title, string? subtitle, List<TrainingPlan> plans, bool showStart)
        {
            Title = title;
            Subtitle = subtitle;
            Plans = plans;
            ShowStart = showStart;
        }
    }
}
