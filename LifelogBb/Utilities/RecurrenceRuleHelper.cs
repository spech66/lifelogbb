namespace LifelogBb.Utilities;

/// <summary>
/// Helpers for normalising iCalendar recurrence rule strings.
/// </summary>
public static class RecurrenceRuleHelper
{
    /// <summary>
    /// Strips the "RRULE:" prefix from a recurrence rule string if present.
    /// <para>
    /// The rrule.js library stores rules as "RRULE:FREQ=MONTHLY;INTERVAL=1".
    /// Ical.Net 4 accepted this prefix; Ical.Net 5 requires only the value
    /// part: "FREQ=MONTHLY;INTERVAL=1".
    /// </para>
    /// </summary>
    public static string Normalize(string rrule) =>
        rrule.StartsWith("RRULE:", StringComparison.OrdinalIgnoreCase)
            ? rrule[6..]
            : rrule;
}
