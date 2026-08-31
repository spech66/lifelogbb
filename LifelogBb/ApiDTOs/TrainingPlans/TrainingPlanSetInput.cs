using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.ApiDTOs.TrainingPlans
{
    public class TrainingPlanSetInput
    {
        [Description("Name of the exercise. Use the same spelling as previous entries so progress can be tracked.")]
        public string? Exercise { get; set; }

        [Description("Planned number of repetitions for this set. Use 0 for a pure hold and set DurationSeconds instead.")]
        public int Reps { get; set; }

        [Description("Planned weight for this set. Leave empty when no weight applies (bodyweight, band, mobility work) -- that is kept distinct from an explicit 0 and excluded from volume statistics.")]
        public double? Weight { get; set; }

        [Description("Planned duration of this set in seconds, for holds and timed work such as planks or stretches. Combine with Reps for timed repetitions, or use it alone with Reps 0 for a single hold.")]
        [Range(1, TrainingSetRules.MaxDurationSeconds)]
        public int? DurationSeconds { get; set; }

        [Description("How many identical sets this entry stands for, so \"3 x 15\" can be sent as one entry instead of three. Expands into that many consecutive sets, each numbered \"Set n/N\" in its notes. Defaults to 1.")]
        [Range(1, TrainingSetRules.MaxRepeat)]
        public int? Repeat { get; set; }

        public string? Notes { get; set; }
    }
}
