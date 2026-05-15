namespace LifelogBb.Models.Entities
{
    public enum Measurements
    {
        Metric = 0,
        Imperial = 1
    }

    /// <summary>
    /// Settings related to the application.
    /// Model generation default values are duplicated in OnModelCreating of LifelogBbContext.
    /// </summary>
    public class Config : BaseEntity
    {
        public DayOfWeek StartOfWeek { get; set; } = DayOfWeek.Monday;

        public Measurements UnitsType { get; set; } = Measurements.Metric;

        public int Height { get; set; } = 170;

        public int BucketListPageSize { get; set; } = 12;

        public int GoalPageSize { get; set; } = 20;

        public int HabitPageSize { get; set; } = 20;

        public int JournalPageSize { get; set; } = 20;

        public int QuotePageSize { get; set; } = 20;

        public int StrengthTrainingPageSize { get; set; } = 20;

        public int EnduranceTrainingPageSize { get; set; } = 20;

        public int TodoPageSize { get; set; } = 20;

        public int WeightPageSize { get; set; } = 20;

        public double WeightWarning { get; set; } = 1.0;

        public string WeightWarningText { get; set; } = "You are gaining weight!";

        public string FeedToken { get; set; } = Guid.NewGuid().ToString();

        public string FeedTimeZone { get; set; } = "Europe/Berlin";

        public string ChatEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

        public string ChatApiKey { get; set; } = "";

        public string ChatModel { get; set; } = "gpt-4o";

        public string ChatSystemPrompt { get; set; } = "You are a helpful life-tracking assistant for LifelogBB. You can query the user's data (weights, journals, todos, goals, habits, quotes, strength trainings, endurance trainings) using the available tools. Summarize and analyze the data to help the user understand their progress and habits.";

        public int ChatMaxToolRoundtrips { get; set; } = 10;

        public Config()
        {
            // Default constructor
        }

        public static Config GetConfig(LifelogBbContext context)
        {
            var config = context.Configs.FirstOrDefault();
            if (config == null)
            {
                config = new Config();
                config.SetCreateFields();
                context.Configs.Add(config);
                context.SaveChanges();
            }
            return config;
        }
    }
}
