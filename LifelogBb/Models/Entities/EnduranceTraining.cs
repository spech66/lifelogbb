using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class EnduranceTraining : BaseEntity
    {
        [Required]
        [MinLength(1)]
        public string? Exercise { get; set; }

        [Display(Name = "Distance (km)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Distance { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? Duration { get; set; } // TimeSpan should work now https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
