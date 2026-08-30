using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.StrengthTrainings
{
    public class EditStrengthTrainingViewModel
    {
        public long Id { get; set; }

        public string? Exercise { get; set; }

        public int Reps { get; set; }

        public double? Weight { get; set; }

        [Range(1, 86400)]
        [Display(Name = "Duration (seconds)")]
        public int? DurationSeconds { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public long? TrainingPlanId { get; set; }

        public long? TrainingPlanSetId { get; set; }
    }
}
