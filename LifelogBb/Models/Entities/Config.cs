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

        /*public int EnduranceTrainingPageSize { get; set; }

        public int GoalPageSize { get; set; }

        public int HabitPageSize { get; set; }

        public int JournalPageSize { get; set; }

        public int QuotePageSize { get; set; }

        public int StrengthTrainingPageSize { get; set; }

        public int TodoPageSize { get; set; }*/

        public int WeightPageSize { get; set; } = 20;

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
