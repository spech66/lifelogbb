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
