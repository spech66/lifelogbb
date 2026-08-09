using LifelogBb.Models.Entities;

namespace LifelogBb.Models.TrainingPlans
{
    // Pairs a planned set with the StrengthTraining row (if any) that fulfilled it. Used by both the
    // Workout capture view (Actual == null means "not done yet") and the read-only Details/Soll-Ist view.
    public class PlanSetComparisonRow
    {
        public TrainingPlanSet Planned { get; set; } = null!;
        public StrengthTraining? Actual { get; set; }
    }

    public class TrainingPlanDetailsViewModel
    {
        public TrainingPlan Plan { get; set; } = null!;
        public List<PlanSetComparisonRow> Rows { get; set; } = new();
        public List<StrengthTraining> ExtraTrainings { get; set; } = new();

        public int DoneCount => Rows.Count(r => r.Actual != null);
        public int TotalCount => Rows.Count;
        public double CompletionRate => TotalCount == 0 ? 0 : (double)DoneCount / TotalCount;
    }

    public class WorkoutViewModel
    {
        public TrainingPlan Plan { get; set; } = null!;
        public List<PlanSetComparisonRow> Rows { get; set; } = new();
        public List<StrengthTraining> ExtraTrainings { get; set; } = new();

        public int DoneCount => Rows.Count(r => r.Actual != null);
        public int TotalCount => Rows.Count;
    }
}
