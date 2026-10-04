using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.EnduranceTrainings
{
    public class EnduranceTrainingInput
    {
        public string? Exercise { get; set; }

        public double Distance { get; set; }

        public TimeSpan? Duration { get; set; }

        public string? Notes { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Description("The day this workout was done. Defaults to today when creating; left unchanged when omitted on update.")]
        public DateTime? Date { get; set; }
    }
}
