using LifelogBb.Models;
using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities
{
    public static class DbSeeder
    {
        public static void Seed(LifelogBbContext context)
        {
            // Ensure all migrations are applied and tables exist
            context.Database.Migrate();

            // Early return if data already exists (checking only a few tables for efficiency)
            if (context.Weights.Any() || context.Journals.Any() || context.Todos.Any())
            {
                return;
            }

            // Add some weights
            var w1 = new Weight(195, 90);
            w1.Bmi = (w1.BodyWeight * 1.0) / (((w1.Height * 0.01) * w1.Height) * 0.01); // Metric BMI calculation
            w1.SetCreateFields();
            context.Weights.Add(w1);
            context.SaveChanges();
        }
    }
}
