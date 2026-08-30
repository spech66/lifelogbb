using LifelogBb.Interfaces.DTOs;
using LifelogBb.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class StrengthTraining : BaseEntity
    {
        [Required]
        [MinLength(1)]
        public string Exercise { get; set; } = string.Empty;

        // 0 for pure holds, where DurationSeconds carries the effort instead.
        public int Reps { get; set; }

        // Null means no weight applies (bodyweight, band, mobility work) as opposed to an actual 0,
        // so volume aggregations can skip the set instead of counting it as zero volume.
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? Weight { get; set; }

        // Set for timed work (planks, stretches, holds) where reps do not describe the effort.
        [Range(1, TrainingSetRules.MaxDurationSeconds)]
        [Display(Name = "Duration (seconds)")]
        public int? DurationSeconds { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        [DefaultValue(3)]
        public int Rating { get; set; }

        // The day this set was trained. Distinct from CreatedAt so sets can be logged retroactively
        // and day plans can be matched to the day they are for.
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Set when this entry was logged against a training plan (template or day plan).
        public long? TrainingPlanId { get; set; }

        // Set when this entry was logged against a specific planned set, enabling planned-vs-actual comparison
        // and letting the workout capture view resume/derive its done state from the database.
        public long? TrainingPlanSetId { get; set; }

        public TrainingPlan? TrainingPlan { get; set; }

        public TrainingPlanSet? TrainingPlanSet { get; set; }

        public StrengthTraining()
        {
            // Default constructor
        }

        public StrengthTraining(string exercise, int reps, double? weight, string notes, int rating)
        {
            Exercise = exercise;
            Reps = reps;
            Weight = weight;
            Notes = notes;
            Rating = rating;
        }
    }
}
