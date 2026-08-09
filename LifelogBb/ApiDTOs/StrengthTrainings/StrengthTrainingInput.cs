using System.ComponentModel;
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

        [Description("The day this set was trained. Defaults to today when omitted.")]
        public DateTime? Date { get; set; }

        [Description("Id of the training plan (template or day plan) this set belongs to, if any.")]
        public long? TrainingPlanId { get; set; }

        [Description("Id of the specific planned set this entry fulfills, if any.")]
        public long? TrainingPlanSetId { get; set; }
    }
}
