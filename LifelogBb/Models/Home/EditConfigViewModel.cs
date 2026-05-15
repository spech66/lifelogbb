using LifelogBb.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Home
{
    public class EditConfigViewModel : IValidatableObject
    {
        public const int HeightMinMetric = 40;
        public const int HeightMaxMetric = 250;
        public const int HeightMinImperial = 16;
        public const int HeightMaxImperial = 98;

        public long Id { get; set; } // For model/view generation only

        public DayOfWeek StartOfWeek { get; set; }

        public Measurements UnitsType { get; set; }

        public int Height { get; set; }

        public string HeightHelpText => UnitsType == Measurements.Metric
            ? HeightHelpTextMetric
            : HeightHelpTextImperial;

        public string HeightHelpTextMetric => $"Metric: {HeightMinMetric}–{HeightMaxMetric} cm";

        public string HeightHelpTextImperial => $"Imperial: {HeightMinImperial}–{HeightMaxImperial} inches";

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

        public string ChatEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

        public string ChatApiKey { get; set; } = "";

        public bool HasChatApiKey { get; set; }

        public string ChatModel { get; set; } = "gpt-4o";

        public string ChatSystemPrompt { get; set; } = "You are a helpful life-tracking assistant for LifelogBB. You can query the user's data (weights, journals, todos, goals, habits, quotes, strength trainings, endurance trainings) using the available tools. Summarize and analyze the data to help the user understand their progress and habits.";

        [Range(1, 50)]
        public int ChatMaxToolRoundtrips { get; set; } = 10;

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
