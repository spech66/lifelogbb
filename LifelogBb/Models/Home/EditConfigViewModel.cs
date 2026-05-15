using LifelogBb.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Home
{
    public class EditConfigViewModel : IValidatableObject
    {
        private const int HeightMinMetric = 40;
        private const int HeightMaxMetric = 250;
        private const int HeightMinImperial = 16;
        private const int HeightMaxImperial = 98;

        public long Id { get; set; } // For model/view generation only

        public DayOfWeek StartOfWeek { get; set; }

        public Measurements UnitsType { get; set; }

        public int Height { get; set; }

        public int BucketListPageSize { get; set; }

        public int GoalPageSize { get; set; }

        public int HabitPageSize { get; set; }

        public int JournalPageSize { get; set; }

        public int QuotePageSize { get; set; }

        public int StrengthTrainingPageSize { get; set; }

        public int EnduranceTrainingPageSize { get; set; }

        public int TodoPageSize { get; set; }

        public int WeightPageSize { get; set; }

        public double WeightWarning { get; set; }

        public string WeightWarningText { get; set; } = "You are gaining weight!";

        public string FeedToken { get; set; } = Guid.NewGuid().ToString();

        public string FeedTimeZone { get; set; } = "Europe/Berlin";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitsType == Measurements.Metric)
            {
                if (Height < HeightMinMetric || Height > HeightMaxMetric)
                {
                    yield return new ValidationResult($"Height must be between {HeightMinMetric} and {HeightMaxMetric} cm in metric mode.", [nameof(Height)]);
                }
            }
            else if (Height < HeightMinImperial || Height > HeightMaxImperial)
            {
                yield return new ValidationResult($"Height must be between {HeightMinImperial} and {HeightMaxImperial} inches in imperial mode.", [nameof(Height)]);
            }
        }
    }
}
