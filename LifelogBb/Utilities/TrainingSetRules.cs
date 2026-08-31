namespace LifelogBb.Utilities
{
    // Limits for the optional parts of a set. The MCP tools call the services directly, so the [Range]
    // attributes on the input DTOs only cover the HTTP path -- these guards are what actually holds on
    // every path, and the attributes reference the same constants so the two cannot drift apart.
    public static class TrainingSetRules
    {
        public const int MaxDurationSeconds = 86400;

        public const int MaxRepeat = 100;

        public static void ValidateDuration(int? durationSeconds)
        {
            if (durationSeconds is null)
                return;

            if (durationSeconds < 1 || durationSeconds > MaxDurationSeconds)
                throw new ArgumentException($"DurationSeconds must be between 1 and {MaxDurationSeconds} but was {durationSeconds}.");
        }

        // Guards against a single request expanding into an unbounded number of rows.
        public static int ValidateRepeat(int? repeat)
        {
            var value = repeat ?? 1;
            if (value < 1 || value > MaxRepeat)
                throw new ArgumentException($"Repeat must be between 1 and {MaxRepeat} but was {value}.");

            return value;
        }

        // A set has to say something about the effort: either repetitions or a duration.
        public static bool HasEffort(int reps, int? durationSeconds) => reps > 0 || durationSeconds is > 0;
    }
}
