using System.Text.RegularExpressions;

namespace LifelogBb.Utilities;

/// <summary>
/// Strips control characters out of untrusted values before they reach the log.
/// Without this, a value containing a newline can forge whole log lines, for example an OAuth
/// client registering itself under the name "ok\nRegistered OAuth client admin".
/// </summary>
public static partial class LogSanitizer
{
    private const int DefaultMaxLength = 200;

    /// <summary>Unicode "other" category: control, format and unassigned characters.</summary>
    [GeneratedRegex(@"\p{C}")]
    private static partial Regex ControlCharacters();

    public static string ForLog(string? value, int maxLength = DefaultMaxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        var sanitized = ControlCharacters().Replace(value, "");
        return sanitized.Length <= maxLength ? sanitized : string.Concat(sanitized.AsSpan(0, maxLength), "...");
    }
}
