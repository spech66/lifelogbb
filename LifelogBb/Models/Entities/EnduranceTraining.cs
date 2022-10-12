using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class EnduranceTraining : BaseEntity
    {
        public string? Exercise { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Distance { get; set; }

        public TimeSpan? Duration { get; set; } // TimeSpan should work now https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
        /*[DisplayName("Duration")]
        public double DurationMilliseconds { get; set; }

        [NotMapped]
        public TimeSpan Duration
        {
            get { return TimeSpan.FromMilliseconds(DurationMilliseconds); }
            set { DurationMilliseconds = value.TotalMilliseconds; }
        }*/

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
