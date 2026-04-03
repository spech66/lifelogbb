using LifelogBb.Models.Entities;

namespace LifelogBb.Models.StrengthTrainings
{
    public class StrengthTrainingPersonalRecord
    {
        public string Exercise { get; set; } = string.Empty;
        public double MaxWeight { get; set; }
        public int MaxReps { get; set; }
        public double MaxVolume { get; set; }
        public int TotalSessions { get; set; }
    }

    public class StrengthTrainingIndexViewModel
    {
        public int TotalSessions { get; set; }
        public int UniqueExerciseCount { get; set; }
        public double TotalVolume { get; set; }
        public StrengthTraining? LastSession { get; set; }
        public List<StrengthTrainingPersonalRecord> PersonalRecords { get; set; } = new();
        public List<StrengthTraining> RecentSessions { get; set; } = new();
    }
}
