using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.TrainingPlans
{
    public class TrainingPlanSetInput
    {
        [Description("Name of the exercise. Use the same spelling as previous entries so progress can be tracked.")]
        public string? Exercise { get; set; }

        [Description("Planned number of repetitions for this set.")]
        public int Reps { get; set; }

        [Description("Planned weight for this set.")]
        public double Weight { get; set; }

        public string? Notes { get; set; }
    }
}
