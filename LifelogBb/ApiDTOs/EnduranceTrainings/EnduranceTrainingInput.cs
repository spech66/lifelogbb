using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.EnduranceTrainings
{
    public class EnduranceTrainingInput
    {
        public string? Exercise { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Distance { get; set; }

        public TimeSpan? Duration { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
