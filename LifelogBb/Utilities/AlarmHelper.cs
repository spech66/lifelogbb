using System.Text.RegularExpressions;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace LifelogBb.Utilities;

/// <summary>
/// A single reminder lead time, for example "15 minutes before".
/// </summary>
/// <param name="Amount">How much time before the anchor date the reminder fires.</param>
/// <param name="Unit">'M' for minutes, 'H' for hours, 'D' for days.</param>
public readonly record struct AlarmOffset(int Amount, char Unit)
{
    /// <summary>Renders the offset as the negative ISO-8601 duration used in TRIGGER.</summary>
    public string ToDurationString() => Unit == 'D' ? $"-P{Amount}D" : $"-PT{Amount}{Unit}";

    /// <summary>Renders the offset as human readable text, for example "15 minutes before".</summary>
    public string ToText() => Unit switch
    {
        'M' => $"{Amount} minute{(Amount == 1 ? "" : "s")} before",
        'H' => $"{Amount} hour{(Amount == 1 ? "" : "s")} before",
        _ => $"{Amount} day{(Amount == 1 ? "" : "s")} before",
    };

    /// <summary>
    /// Converts the offset to the negative <see cref="Duration"/> expected by Ical.Net.
    /// Days stay nominal ("-P1D") instead of being expanded to hours.
    /// </summary>
    public Duration ToNegativeDuration() => Unit switch
    {
        'M' => Duration.FromMinutes(-Amount),
        'H' => Duration.FromHours(-Amount),
        _ => Duration.FromDays(-Amount),
    };
}

/// <summary>
/// Helpers for the reminder lead times stored on habits, todos and goals.
/// <para>
/// Lead times are persisted as a comma separated list of negative ISO-8601 durations,
/// exactly as they appear in an iCalendar TRIGGER property: "-PT15M,-PT2H,-P1D".
/// A null or empty value means the entry has no reminders.
/// </para>
/// </summary>
public static class AlarmHelper
{
    /// <summary>Maximum number of reminders kept per entry.</summary>
    public const int MaxAlarms = 10;

    /// <summary>
    /// Validation pattern for the stored list. Declared as a const so it can be passed to
    /// <see cref="System.ComponentModel.DataAnnotations.RegularExpressionAttribute"/>.
    /// </summary>
    public const string Pattern = @"^\s*$|^\s*-P(T\d{1,4}[HM]|\d{1,4}D)(\s*,\s*-P(T\d{1,4}[HM]|\d{1,4}D))*\s*$";

    private static readonly Regex EntryRegex = new(
        @"^-P(?:T(?<amount>\d{1,4})(?<unit>[HM])|(?<amount>\d{1,4})(?<unit>D))$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Parses the stored list into offsets. Entries that cannot be read are skipped rather than
    /// throwing, so bad data can never break a calendar feed. Duplicates are dropped and the
    /// result is capped at <see cref="MaxAlarms"/>.
    /// </summary>
    public static IReadOnlyList<AlarmOffset> Parse(string? alarms)
    {
        if (string.IsNullOrWhiteSpace(alarms))
        {
            return Array.Empty<AlarmOffset>();
        }

        var offsets = new List<AlarmOffset>();
        foreach (var part in alarms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var match = EntryRegex.Match(part);
            if (!match.Success)
            {
                continue;
            }

            var amount = int.Parse(match.Groups["amount"].Value);
            if (amount <= 0)
            {
                continue;
            }

            var offset = new AlarmOffset(amount, match.Groups["unit"].Value[0]);
            if (!offsets.Contains(offset))
            {
                offsets.Add(offset);
            }

            if (offsets.Count >= MaxAlarms)
            {
                break;
            }
        }

        return offsets;
    }

    /// <summary>Human readable text for every reminder, used by the display template.</summary>
    public static IEnumerable<string> ToTextParts(string? alarms) => Parse(alarms).Select(offset => offset.ToText());

    /// <summary>
    /// Builds DISPLAY alarms for a calendar component.
    /// </summary>
    /// <param name="alarms">The stored reminder list.</param>
    /// <param name="summary">Text shown by the calendar client when the reminder fires.</param>
    /// <param name="relation">
    /// <see cref="TriggerRelation.Start"/> to trigger relative to DTSTART (events) or
    /// <see cref="TriggerRelation.End"/> to trigger relative to DUE (todos).
    /// </param>
    public static IEnumerable<Alarm> BuildAlarms(string? alarms, string? summary, string relation)
    {
        var description = string.IsNullOrWhiteSpace(summary) ? "Reminder" : summary;

        return Parse(alarms).Select(offset =>
        {
            var trigger = new Trigger()
            {
                Duration = offset.ToNegativeDuration(),
                Related = relation,
            };

            // Ical.Net 5.2.1: setting Trigger.Related alone does not emit the RELATED=
            // parameter (CalendarSerializer only reads the Parameters collection), so it
            // must be set explicitly too. Only needed for END; START is the RFC default.
            if (relation == TriggerRelation.End)
            {
                trigger.Parameters.Set("RELATED", relation);
            }

            // RFC 5545 3.6.6: a DISPLAY alarm must carry ACTION, TRIGGER and DESCRIPTION
            return new Alarm()
            {
                Action = AlarmAction.Display,
                Summary = description,
                Description = description,
                Trigger = trigger,
            };
        });
    }
}
