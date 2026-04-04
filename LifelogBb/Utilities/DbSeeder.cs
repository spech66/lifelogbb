using LifelogBb.Models;
using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities
{
    public static class DbSeeder
    {
        /// <summary>
        /// Generate some test data.
        /// Uses the current data and generates some data for older years to make the app look more alive.
        /// </summary>
        /// <param name="context"></param>
        public static void Seed(LifelogBbContext context)
        {
            // Ensure all migrations are applied and tables exist
            context.Database.Migrate();

            // Early return if data already exists (checking only a few tables for efficiency)
            if (context.Weights.Any() || context.Journals.Any() || context.Todos.Any())
            {
                return;
            }

            AddTestBucketList(context);
            AddTestEnduranceTraining(context);
            AddTestGoal(context);
            AddTestHabit(context);
            AddTestJournal(context);
            AddTestQuote(context);
            AddTestStrengthTraining(context);
            AddTestTodo(context);
            AddTestWeights(context);
        }

        private static void AddTestBucketList(LifelogBbContext context)
        {
            // TODO: Add test data for BucketList
        }

        private static void AddTestEnduranceTraining(LifelogBbContext context)
        {
            // TODO: Add test data for EnduranceTraining
        }

        private static void AddTestGoal(LifelogBbContext context)
        {
            // TODO: Add test data for Goal
        }

        private static void AddTestHabit(LifelogBbContext context)
        {
            // TODO: Add test data for Habit
        }

        private static void AddTestJournal(LifelogBbContext context)
        {
            // TODO: Add test data for Journal
        }

        private static void AddTestQuote(LifelogBbContext context)
        {
            // TODO: Add test data for Quote
        }

        private static void AddTestStrengthTraining(LifelogBbContext context)
        {
            // TODO: Add test data for StrengthTraining
        }

        private static void AddTestTodo(LifelogBbContext context)
        {
            // TODO: Add test data for Todo
        }

        /// <summary>
        /// Weight loss, gain, loss over 5 years. Tracking on several days a month.
        /// </summary>
        /// <param name="context"></param>
        private static void AddTestWeights(LifelogBbContext context)
        {
            var currentYear = DateTime.Now.Year;
            for(int year = currentYear - 5; year <= currentYear; year++)
            {
                int height = 192;
                for(int month = 1; month <= 12; month++)
                {
                    // Randomly decide to skip some months to create a more realistic dataset
                    if (new Random().Next(0, 2) == 0) continue;

                    var day = new Random().Next(1, 28);

                    // Simulate weight changes over years going down to 85 Kg
                    var weightValue = 85 - ((year - currentYear) * 5) + new Random().Next(-3, 4); // Random fluctuation of ±3 kg
                    var weightEntry = new Weight(height, weightValue)
                    {
                        Bmi = (weightValue * 1.0) / (((height * 0.01) * height) * 0.01), // Metric BMI calculation
                        CreatedAt = new DateTime(year, month, day)
                    };
                    context.Weights.Add(weightEntry);
                }
            }

            context.SaveChanges();
        }
    }
}
