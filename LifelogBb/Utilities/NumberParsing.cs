using System.Globalization;

namespace LifelogBb.Utilities;

/// <summary>
/// Number parsing that does not depend on the culture the server happens to run under.
/// </summary>
public static class NumberParsing
{
    private const NumberStyles Styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Parses a double written with either the English "1.5" or the German "1,5" decimal separator,
    /// with the same result on an en and a de machine.
    /// </summary>
    /// <remarks>
    /// A single "." or "," is always read as the decimal separator, so the same input gives the same
    /// number under every culture. Digit grouping is not supported and cannot be: "1,000" is one
    /// thousand in en and one in de, so it is read as 1.0 here rather than guessed. Values carrying
    /// more than one separator ("1,234.56", "1.234.567") are rejected as ambiguous.
    /// </remarks>
    public static bool TryParseDouble(string? value, out double result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        var dots = trimmed.Count(c => c == '.');
        var commas = trimmed.Count(c => c == ',');

        // Both separators, or one of them repeated, means digit grouping is definitely in play and
        // the value cannot be read the same way in both cultures.
        if ((dots > 0 && commas > 0) || dots > 1 || commas > 1)
        {
            return false;
        }

        return double.TryParse(trimmed.Replace(',', '.'), Styles, CultureInfo.InvariantCulture, out result);
    }
}
