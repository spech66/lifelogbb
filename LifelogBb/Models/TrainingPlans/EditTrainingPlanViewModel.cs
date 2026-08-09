using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.TrainingPlans
{
    // One row as edited in the plan set editor (wwwroot/js/plansetseditor.js), serialized to/from the
    // hidden SetsJson field below. SortOrder is not part of it -- array order is the order.
    public class PlanSetRow
    {
        public string Exercise { get; set; } = string.Empty;
        public int Reps { get; set; }
        public double Weight { get; set; }
        public string? Notes { get; set; }
    }

    public class EditTrainingPlanViewModel
    {
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        public bool IsArchived { get; set; }

        // Serialized List<PlanSetRow> JSON, populated/consumed by plansetseditor.js.
        public string SetsJson { get; set; } = "[]";
    }
}
