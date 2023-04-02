using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class Goal : BaseEntityTagged
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? TargetValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? CurrentValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }

        public Goal()
        {
            // Default constructor
        }

        public Goal(string name, string description, double targetValue, double currentValue, DateTime? startDate, DateTime? endDate, bool isCompleted)
        {
            Name = name;
            Description = description;
            TargetValue = targetValue;
            CurrentValue = currentValue;
            StartDate = startDate;
            EndDate = endDate;
            IsCompleted = isCompleted;
        }
    }
}
