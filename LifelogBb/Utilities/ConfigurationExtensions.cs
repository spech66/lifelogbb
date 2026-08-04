namespace LifelogBb.Utilities;

/// <summary>
/// Helpers for reading configuration values that the application cannot run without.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Returns the value for <paramref name="key"/> and throws when it is missing or empty.
    /// Turns a missing setting into a clear error naming the key instead of a null reference
    /// somewhere further down.
    /// </summary>
    public static string GetRequired(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Required configuration value \"{key}\" is missing.");
        }

        return value;
    }

    /// <summary>
    /// Returns the value for <paramref name="key"/> as a number and throws when it is missing or
    /// not a valid number. Accepts both "1.5" and "1,5" so the same appsettings file works on an
    /// en and a de machine.
    /// </summary>
    public static double GetRequiredDouble(this IConfiguration configuration, string key)
    {
        var value = configuration.GetRequired(key);
        if (!NumberParsing.TryParseDouble(value, out var number))
        {
            throw new InvalidOperationException($"Configuration value \"{key}\" is not a valid number: \"{value}\".");
        }

        return number;
    }
}
