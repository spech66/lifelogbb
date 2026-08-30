using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LifelogBb.Utilities;

namespace LifelogBb.ApiDTOs.StrengthTrainings
{
    public class StrengthTrainingInput
    {
        public string? Exercise { get; set; }

        [Description("Number of repetitions. Use 0 for a pure hold and set DurationSeconds instead.")]
        public int Reps { get; set; }

        [Description("Weight used for this set. Leave empty when no weight applies (bodyweight, band, mobility work) -- that is kept distinct from an explicit 0 and excluded from volume statistics.")]
        public double? Weight { get; set; }

        [Description("Duration of this set in seconds, for holds and timed work such as planks or stretches. Combine with Reps for timed repetitions, or use it alone with Reps 0 for a single hold.")]
        [Range(1, TrainingSetRules.MaxDurationSeconds)]
        public int? DurationSeconds { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Description("The day this set was trained. Defaults to today when omitted.")]
        public DateTime? Date { get; set; }

        [Description("Id of the training plan (template or day plan) this set belongs to, if any.")]
        public long? TrainingPlanId { get; set; }

        [Description("Id of the specific planned set this entry fulfills, if any.")]
        public long? TrainingPlanSetId { get; set; }
    }
}
