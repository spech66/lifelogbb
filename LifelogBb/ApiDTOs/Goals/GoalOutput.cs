using LifelogBb.Interfaces.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Goals
{
    public class GoalOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? TargetValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? CurrentValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCompleted { get; set; }

        public string? Category { get; set; }

        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
