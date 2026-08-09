using LifelogBb.Interfaces.DTOs;
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

        public int Reps { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Weight { get; set; }

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

        public StrengthTraining(string exercise, int reps, double weight, string notes, int rating)
        {
            Exercise = exercise;
            Reps = reps;
            Weight = weight;
            Notes = notes;
            Rating = rating;
        }
    }
}
