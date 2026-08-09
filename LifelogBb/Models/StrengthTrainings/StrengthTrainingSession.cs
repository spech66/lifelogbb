namespace LifelogBb.Models.StrengthTrainings
{
    // One row of the Sessions overview: all sets trained on a single day, grouped together.
    public class StrengthTrainingSession
    {
        public DateTime Date { get; set; }
        public int SetCount { get; set; }
        public double Volume { get; set; }
        public List<string> Exercises { get; set; } = new();
        public long? TrainingPlanId { get; set; }
        public string? TrainingPlanName { get; set; }
        public int? PlannedSetCount { get; set; }

        public double? CompletionRate => PlannedSetCount is > 0 ? Math.Min(1.0, (double)SetCount / PlannedSetCount.Value) : null;
    }
}
