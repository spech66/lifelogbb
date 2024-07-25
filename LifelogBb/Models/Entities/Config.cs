using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
