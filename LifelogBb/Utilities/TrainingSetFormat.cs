using LifelogBb.Models.Entities;

namespace LifelogBb.Utilities
{
    // One place to render a set as text, because reps, weight and duration are each optional and every
    // combination of them shows up somewhere: "10 x 20 kg" for classic strength work, "10 reps" for
    // bodyweight, "45 s" for a plank, "10 reps - 30 s" for timed reps.
    public static class TrainingSetFormat
    {
        public const string Empty = "-";

        public static string Describe(int reps, double? weight, int? durationSeconds)
        {
            string? core = null;

            if (reps > 0 && weight.HasValue)
                core = $"{reps} × {weight.Value.ToString("0.##")} kg";
            else if (reps > 0)
                core = $"{reps} reps";
            else if (weight.HasValue)
                core = $"{weight.Value.ToString("0.##")} kg";

            var duration = durationSeconds is > 0 ? FormatDuration(durationSeconds.Value) : null;

            if (core == null)
                return duration ?? Empty;

            return duration == null ? core : $"{core} · {duration}";
        }

        public static string Describe(TrainingPlanSet set) => Describe(set.Reps, set.Weight, set.DurationSeconds);

        public static string Describe(StrengthTraining training) => Describe(training.Reps, training.Weight, training.DurationSeconds);

        // Seconds stay seconds while they read naturally; beyond a minute they turn into "2 min" or "1:30 min".
        public static string FormatDuration(int seconds)
        {
            if (seconds < 60)
                return $"{seconds} s";

            var minutes = seconds / 60;
            var rest = seconds % 60;
            return rest == 0 ? $"{minutes} min" : $"{minutes}:{rest:00} min";
        }

        // Volume only means something when a weight was actually moved.
        public static double? Volume(int reps, double? weight) => weight.HasValue ? reps * weight.Value : null;
    }
}
