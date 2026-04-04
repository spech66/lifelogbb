using LifelogBb.Models;
using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities
{
    public static class DbSeeder
    {
        private static readonly string[] Categories = new[] { "Health", "Fitness", "Personal Development", "Work", "Hobbies" };

        private static readonly string[] Tags = new[] { "Motivation", "Progress", "Challenges", "Reflection", "Dancing", "Cooking", "Travel", "Learning", "Relaxation" };

        private static readonly string DoggoIpsum = "Doggo ipsum porgo. Blep big ol pupper very good spot long bois, fat boi heckin good boys and girls. what a nice floof you are doing me a frighten. Heckin good boys and girls stop it fren big ol much ruin diet porgo very taste wow puggo mlem, waggy wags ur givin me a spook big ol aqua doggo heck. Big ol pupper sub woofer puggo very jealous pupper floofs, wow such tempt bork. wow very biscit smol borking doggo with a long snoot for pats. Blep big ol noodle horse puggo, fat boi. Doggorino pupperino waggy wags heckin, bork. You are doing me a frighten h*ck doing me a frighten wow very biscit fluffer, length boy shibe. Many pats smol borking doggo with a long snoot for pats corgo pats, much ruin diet borkdrive. Big ol very good spot noodle horse, heck. Mlem long doggo wow very biscit, dat tungg tho.\r\n\r\nBorkdrive floofs very jealous pupper doge much ruin diet stop it fren such treat, I am bekom fat boof very hand that feed shibe adorable doggo noodle horse. Pupperino aqua doggo ruff, waggy wags. Smol borking doggo with a long snoot for pats very hand that feed shibe the neighborhood pupper long bois, waggy wags. Snoot you are doing me a frighten floofs corgo pupper very hand that feed shibe, woofer pats lotsa pats most angery pupper I have ever seen. Thicc maximum borkdrive shibe smol floofs noodle horse what a nice floof, you are doing me the shock blep porgo I am bekom fat. Heckin angery woofer borkf you are doing me a frighten corgo, shooberino. Blop super chub what a nice floof smol borking doggo with a long snoot for pats length boy snoot wow very biscit, doggorino bork floofs long water shoob.  Floofs long water shoob vvv length boy snoot, bork porgo.\r\n\r\nyou are doing me the shock boof. Heckin good boys pats long bois very good spot doggorino, aqua doggo I am bekom fat doing me a frighten aqua doggo thicc, aqua doggo he made many woofs bork. Very hand that feed shibe aqua doggo you are doing me the shock shoob heckin wow such tempt very hand that feed shibe, very good spot long bois smol very good spot. Heckin angery woofer ruff yapper length boy, heckin angery woofer vvv.";

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
            var imageIds = new[]
            {
                "5edae359-8204-4111-900f-eef627c78188",
                "5fabdab9-b303-42cf-8bf5-87ca51a04268",
                "70940846-5101-406f-a3a3-8b1afbc83f78",
                "734e2d81-ec5a-46c3-b8cc-34a8c4f89ee3"
            };

            var bucketListItems = new List<BucketList>();
            var now = DateTime.Now;
            var statuses = Enum.GetValues(typeof(BucketListStatus));
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                var item = new BucketList
                {
                    Title = $"Bucket List Item {i + 1}",
                    Description = DoggoIpsum.Substring(0, rand.Next(50, Math.Min(200, DoggoIpsum.Length))),
                    CreatedAt = now.AddDays(-i * 30),
                    UpdatedAt = now.AddDays(-i * 30),
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] }),
                    Status = (BucketListStatus)statuses.GetValue(rand.Next(statuses.Length))!,
                    ImageFileName = i < imageIds.Length ? imageIds[i] : null,
                    ImageName = i < imageIds.Length ? "test.png" : null,
                };
                bucketListItems.Add(item);
            }

            context.BucketLists.AddRange(bucketListItems);
            context.SaveChanges();
        }

        private static void AddTestEnduranceTraining(LifelogBbContext context)
        {
            var now = DateTime.Now;
            var rand = new Random();
            var enduranceList = new List<EnduranceTraining>
            {
                new EnduranceTraining
                {
                    Exercise = "Running",
                    Distance = 5.0,
                    Duration = TimeSpan.FromMinutes(28),
                    Notes = "Morning run in the park.",
                    Rating = 4,
                    CreatedAt = now.AddDays(-10),
                    UpdatedAt = now.AddDays(-10)
                },
                new EnduranceTraining
                {
                    Exercise = "Cycling",
                    Distance = 20.5,
                    Duration = TimeSpan.FromMinutes(65),
                    Notes = "Road cycling with friends.",
                    Rating = 5,
                    CreatedAt = now.AddDays(-7),
                    UpdatedAt = now.AddDays(-7)
                },
                new EnduranceTraining
                {
                    Exercise = "Swimming",
                    Distance = 1.2,
                    Duration = TimeSpan.FromMinutes(35),
                    Notes = "Indoor pool laps.",
                    Rating = 3,
                    CreatedAt = now.AddDays(-5),
                    UpdatedAt = now.AddDays(-5)
                },
                new EnduranceTraining
                {
                    Exercise = "Walking",
                    Distance = 3.0,
                    Duration = TimeSpan.FromMinutes(40),
                    Notes = "Evening walk with dog.",
                    Rating = 4,
                    CreatedAt = now.AddDays(-3),
                    UpdatedAt = now.AddDays(-3)
                },
                new EnduranceTraining
                {
                    Exercise = "Running",
                    Distance = 10.0,
                    Duration = TimeSpan.FromMinutes(60),
                    Notes = "Long run, felt great!",
                    Rating = 5,
                    CreatedAt = now.AddDays(-1),
                    UpdatedAt = now.AddDays(-1)
                }
            };

            context.EnduranceTrainings.AddRange(enduranceList);
            context.SaveChanges();
        }

        private static void AddTestGoal(LifelogBbContext context)
        {
            var now = DateTime.Now;
            var rand = new Random();
            var goals = new List<Goal>
            {
                // Not started
                new Goal
                {
                    Name = "Run a Marathon",
                    Description = "Train to run a full marathon.",
                    InitialValue = 0,
                    TargetValue = 42.195,
                    CurrentValue = 0,
                    StartDate = now.AddDays(5),
                    EndDate = now.AddMonths(6),
                    IsCompleted = false,
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                // In progress
                new Goal
                {
                    Name = "Read 12 Books",
                    Description = "Read one book per month.",
                    InitialValue = 0,
                    TargetValue = 12,
                    CurrentValue = 5,
                    StartDate = now.AddMonths(-5),
                    EndDate = now.AddMonths(7),
                    IsCompleted = false,
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                // Completed
                new Goal
                {
                    Name = "Lose 10 Kg",
                    Description = "Reach target weight.",
                    InitialValue = 95,
                    TargetValue = 85,
                    CurrentValue = 85,
                    StartDate = now.AddMonths(-12),
                    EndDate = now.AddMonths(-6),
                    IsCompleted = true,
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                // Overdue
                new Goal
                {
                    Name = "Save $5000",
                    Description = "Save up for a vacation.",
                    InitialValue = 0,
                    TargetValue = 5000,
                    CurrentValue = 2000,
                    StartDate = now.AddMonths(-8),
                    EndDate = now.AddMonths(-1),
                    IsCompleted = false,
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                // Nearly finished
                new Goal
                {
                    Name = "Write a Blog Series",
                    Description = "Publish 10 blog posts.",
                    InitialValue = 0,
                    TargetValue = 10,
                    CurrentValue = 9,
                    StartDate = now.AddMonths(-2),
                    EndDate = now.AddMonths(1),
                    IsCompleted = false,
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                }
            };

            foreach(var goal in goals)
            {
                goal.SetCreateFields();
            }

            context.Goals.AddRange(goals);
            context.SaveChanges();
        }

        private static void AddTestHabit(LifelogBbContext context)
        {
            var now = DateTime.Now;
            var rand = new Random();
            var habits = new List<Habit>
            {
                new Habit
                {
                    Name = "Drink Water",
                    Description = "Drink at least 2 liters of water.",
                    StartDate = now.AddDays(-30),
                    EndDate = now.AddDays(-30).AddMinutes(5),
                    RecurrenceRules = "FREQ=DAILY",
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                new Habit
                {
                    Name = "Morning Walk",
                    Description = "Go for a 20-minute walk every morning.",
                    StartDate = now.AddDays(-20),
                    EndDate = now.AddDays(-20).AddMinutes(20),
                    RecurrenceRules = "FREQ=DAILY",
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                new Habit
                {
                    Name = "Read a Book",
                    Description = "Read at least 10 pages.",
                    StartDate = now.AddDays(-10),
                    EndDate = now.AddDays(-10).AddMinutes(30),
                    RecurrenceRules = "FREQ=DAILY",
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                new Habit
                {
                    Name = "Weekly Review",
                    Description = "Review your week and plan ahead.",
                    StartDate = now.AddDays(-21),
                    EndDate = now.AddDays(-21).AddMinutes(30),
                    RecurrenceRules = "FREQ=WEEKLY",
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                },
                new Habit
                {
                    Name = "Call Family",
                    Description = "Call a family member every other day.",
                    StartDate = now.AddDays(-14),
                    EndDate = now.AddDays(-14).AddMinutes(20),
                    RecurrenceRules = "FREQ=DAILY;INTERVAL=2",
                    Category = Categories[rand.Next(Categories.Length)],
                    Tags = string.Join(",", new List<string> { Tags[rand.Next(Tags.Length)], Tags[rand.Next(Tags.Length)] })
                }
            };

            foreach(var habit in habits)
            {
                habit.SetCreateFields();
            }

            context.Habits.AddRange(habits);
            context.SaveChanges();
        }

        private static void AddTestJournal(LifelogBbContext context)
        {
            var currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear; year++)
            {
                for(int i = 0; i < 20; i++)
                {
                    var month = new Random().Next(1, 13);
                    var day = new Random().Next(1, 28);
                    var journalEntry = new Journal
                    {
                        Text = DoggoIpsum.Substring(0, new Random().Next(100, DoggoIpsum.Length)),
                        Date = new DateTime(year, month, day),
                        CreatedAt = new DateTime(year, month, day),
                        UpdatedAt = new DateTime(year, month, day),
                        Category = Categories[new Random().Next(Categories.Length)],
                        Tags = String.Join(",", new List<string> { Tags[new Random().Next(Tags.Length)], Tags[new Random().Next(Tags.Length)] }),
                    };

                    // Make sure that date has no duplicate entries
                    if(context.Journals.Any(j => j.Date.Date == journalEntry.Date.Date))
                    {
                        continue;
                    }
                    context.Journals.Add(journalEntry);
                }
            }

            context.SaveChanges();
        }

        private static void AddTestQuote(LifelogBbContext context)
        {
            var quotes = new List<Quote>
            {
                new Quote { Text = "The only way to do great work is to love what you do.", Author = "Steve Jobs", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Quote { Text = "Success is not the key to happiness. Happiness is the key to success. If you love what you are doing, you will be successful.", Author = "Albert Schweitzer", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Quote { Text = "Don't watch the clock; do what it does. Keep going.", Author = "Sam Levenson", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Quote { Text = "The future belongs to those who believe in the beauty of their dreams.", Author = "Eleanor Roosevelt", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Quote { Text = "It does not matter how slowly you go as long as you do not stop.", Author = "Confucius", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            };

            foreach (var quote in quotes)
            {
                quote.Category = Categories[new Random().Next(Categories.Length)];
                quote.Tags = String.Join(",", new List<string> { Tags[new Random().Next(Tags.Length)], Tags[new Random().Next(Tags.Length)] });
            }

            context.Quotes.AddRange(quotes);
            context.SaveChanges();
        }

        private static void AddTestStrengthTraining(LifelogBbContext context)
        {
            var now = DateTime.Now;
            var trainings = new List<StrengthTraining>();
            for (int i = 0; i < 5; i++)
            {
                trainings.Add(new StrengthTraining
                {
                    Exercise = "Squats",
                    Reps = 5 + new Random().Next(-1, 2),
                    Weight = 90.0 + 5 * new Random().Next(-2, 2),
                    Notes = "Training Ipsum",                    
                    Rating = new Random().Next(1, 5),
                    CreatedAt = now.AddDays(-i),
                    UpdatedAt = now.AddDays(-i)
                });
                trainings.Add(new StrengthTraining
                {
                    Exercise = "Squats",
                    Reps = 5 + new Random().Next(-1, 2),
                    Weight = 90.0 + 5 * new Random().Next(-2, 2),
                    Notes = "Training Ipsum",
                    Rating = new Random().Next(1, 5),
                    CreatedAt = now.AddDays(-i).AddMinutes(4), // Hopefully no one calls this before midnight
                    UpdatedAt = now.AddDays(-i).AddMinutes(4),
                });
            }
            context.StrengthTrainings.AddRange(trainings);
            context.SaveChanges();
        }

        private static void AddTestTodo(LifelogBbContext context)
        {
            var todos = new List<Todo>
            {
                new Todo { Title = "Go for a run", IsCompleted = false, StartDate = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Todo { Title = "Read a book", IsCompleted = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Todo { Title = "Cook a new recipe", IsCompleted = false, Progress = 10, StartDate = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Todo { Title = "Meditate for 10 minutes", IsCompleted = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Todo { Title = "Call a friend", IsCompleted = false, IsImportant = true, DueDate = DateTime.Now.AddDays(-2), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            };

            foreach (var todo in todos)
            {
                todo.Description = DoggoIpsum.Substring(0, new Random().Next(50, DoggoIpsum.Length));
                todo.Category = Categories[new Random().Next(Categories.Length)];
                todo.Tags = String.Join(",", new List<string> { Tags[new Random().Next(Tags.Length)], Tags[new Random().Next(Tags.Length)] });
            }

            context.Todos.AddRange(todos);
            context.SaveChanges();
        }

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
                        CreatedAt = new DateTime(year, month, day),
                        UpdatedAt = new DateTime(year, month, day)                        
                    };
                    context.Weights.Add(weightEntry);
                }
            }

            context.SaveChanges();
        }
    }
}
