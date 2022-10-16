using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.StrengthTrainings
{
    public class StrengthTrainingInput
    {
        public string? Exercise { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
