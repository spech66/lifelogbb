using LifelogBb.Models.Entities;

namespace LifelogBb.Models.EnduranceTrainings
{
    public class EnduranceTrainingPersonalRecord
    {
        public string Exercise { get; set; } = string.Empty;
        public double BestDistance { get; set; }
        public double? BestPace { get; set; }
        public int TotalSessions { get; set; }
        public double TotalDistance { get; set; }
    }

    public class EnduranceTrainingIndexViewModel
    {
        public int TotalSessions { get; set; }
        public int UniqueExerciseCount { get; set; }
        public double TotalDistance { get; set; }
        public EnduranceTraining? LastSession { get; set; }
        public List<EnduranceTrainingPersonalRecord> PersonalRecords { get; set; } = new();
        public List<EnduranceTraining> RecentSessions { get; set; } = new();
    }
}
