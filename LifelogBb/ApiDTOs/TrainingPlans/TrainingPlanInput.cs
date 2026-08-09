using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.TrainingPlans
{
    public class TrainingPlanInput
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Description("Leave empty for a reusable base/template plan. Set a date to make this a concrete plan for that day.")]
        public DateTime? Date { get; set; }

        public bool IsArchived { get; set; }

        [Description("The planned sets in the order they should be performed. This fully replaces the plan's sets on update -- any set not included here is removed, and links from previously logged strength trainings to a removed set are cleared.")]
        public List<TrainingPlanSetInput> Sets { get; set; } = new();
    }
}
