using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using LifelogBb.Models.Home;
using LifelogBb.Models.Journals;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LifelogBb.Controllers
{
    public class HomeController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;

        public HomeController(LifelogBbContext context, IMapper mapper, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel();

            var weights = await _context.Weights.OrderByDescending(o => o.CreatedAt).Take(10).ToListAsync();
            model.WeightList = weights.OrderBy(o => o.CreatedAt).ToList();

            var enduranceTraining = await _context.EnduranceTrainings.OrderByDescending(o => o.CreatedAt).Take(1).FirstOrDefaultAsync();
            model.LastEnduranceTraining = enduranceTraining;

            var strengthTraining = await _context.StrengthTrainings.OrderByDescending(o => o.CreatedAt).Take(1).FirstOrDefaultAsync();
            model.LastStrengthTraining = strengthTraining;

            var randomBucketList = await _context.BucketLists.OrderBy(r => EF.Functions.Random()).Take(1).FirstOrDefaultAsync();
            model.RandomBucketList = randomBucketList;

            var randomQuote = await _context.Quotes.OrderBy(r => EF.Functions.Random()).Take(1).FirstOrDefaultAsync();
            model.RandomQuote = randomQuote;

            var journals = await _context.Journals.OrderByDescending(o => o.CreatedAt).Take(5).ToListAsync();
            var activities = new List<DashboardViewModelActivity>();
            activities.AddRange(journals.ConvertAll(j => new DashboardViewModelActivity { Type = EntityType.Journal, Text = j.Text.Length > 200 ? j.Text.Substring(0, 200) + "..." : j.Text, Date = j.CreatedAt }));
            model.Activities = activities;

            model.TodoList = await _context.Todos.Where(t => !t.IsCompleted).OrderByDescending(o => o.DueDate).ThenByDescending(o => o.IsImportant).Take(10).ToListAsync();

            model.GoalList = await _context.Goals.Where(t => !t.IsCompleted).OrderByDescending(o => o.EndDate).ThenByDescending(o => o.StartDate).Take(10).ToListAsync();

            var allHabits = await _context.Habits.Where(t => !t.IsCompleted && t.RecurrenceRules != null && t.RecurrenceRules != "" && t.StartDate != null && t.EndDate != null).ToListAsync();
            model.HabitList = calculateHabits(allHabits, DateTime.Now, DateTime.Now.AddDays(7));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Summary(SummaryGranularity? granularity, DateTime? start, DateTime? end)
        {
            var model = new SummaryViewModel
            {
                Granularity = granularity ?? SummaryGranularity.Day
            };

            var today = DateTime.Today;
            var anchor = (start ?? today).Date;

            switch (model.Granularity)
            {
                case SummaryGranularity.Day:
                    model.Start = anchor;
                    model.End = anchor;
                    break;
                case SummaryGranularity.Week:
                    model.Start = anchor.AddDays(-(int)anchor.DayOfWeek);
                    model.End = model.Start.AddDays(6);
                    break;
                case SummaryGranularity.Month:
                    model.Start = new DateTime(anchor.Year, anchor.Month, 1);
                    model.End = model.Start.AddMonths(1).AddDays(-1);
                    break;
                case SummaryGranularity.Year:
                    model.Start = new DateTime(anchor.Year, 1, 1);
                    model.End = model.Start.AddYears(1).AddDays(-1);
                    break;
                case SummaryGranularity.All:
                    model.Start = today;
                    model.End = today;
                    break;
                case SummaryGranularity.Custom:
                    model.Start = (start ?? today).Date;
                    model.End = (end ?? model.Start).Date;
                    if (model.End < model.Start)
                    {
                        (model.Start, model.End) = (model.End, model.Start);
                    }
                    break;
            }

            var queryStart = model.Granularity == SummaryGranularity.All ? DateTime.MinValue : model.Start.Date;
            var queryEndExclusive = model.Granularity == SummaryGranularity.All ? DateTime.MaxValue : model.End.Date.AddDays(1);

            model.Weights = await _context.Weights
                .Where(w => w.CreatedAt >= queryStart && w.CreatedAt < queryEndExclusive)
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();
            model.Todos = await _context.Todos
                .Where(t => t.CreatedAt >= queryStart && t.CreatedAt < queryEndExclusive)
                .OrderByDescending(t => t.IsImportant)
                .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                .ToListAsync();
            model.Goals = await _context.Goals
                .Where(g => g.CreatedAt >= queryStart && g.CreatedAt < queryEndExclusive)
                .OrderBy(g => g.EndDate ?? DateTime.MaxValue)
                .ThenBy(g => g.Name)
                .ToListAsync();
            model.Habits = await _context.Habits
                .Where(h => h.CreatedAt >= queryStart && h.CreatedAt < queryEndExclusive)
                .OrderBy(h => h.StartDate ?? h.CreatedAt)
                .ThenBy(h => h.Name)
                .ToListAsync();
            model.StrengthTrainings = await _context.StrengthTrainings
                .Where(s => s.CreatedAt >= queryStart && s.CreatedAt < queryEndExclusive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            model.EnduranceTrainings = await _context.EnduranceTrainings
                .Where(e => e.CreatedAt >= queryStart && e.CreatedAt < queryEndExclusive)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            model.BucketLists = await _context.BucketLists
                .Where(b => b.CreatedAt >= queryStart && b.CreatedAt < queryEndExclusive)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            model.Quotes = await _context.Quotes
                .Where(q => q.CreatedAt >= queryStart && q.CreatedAt < queryEndExclusive)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            model.Journals = await _context.Journals
                .Where(j => j.CreatedAt >= queryStart && j.CreatedAt < queryEndExclusive)
                .OrderByDescending(j => j.Date)
                .ToListAsync();

            if (model.Granularity == SummaryGranularity.All)
            {
                var allDates = model.Weights.Select(x => x.CreatedAt)
                    .Concat(model.Todos.Select(x => x.CreatedAt))
                    .Concat(model.Goals.Select(x => x.CreatedAt))
                    .Concat(model.Habits.Select(x => x.CreatedAt))
                    .Concat(model.StrengthTrainings.Select(x => x.CreatedAt))
                    .Concat(model.EnduranceTrainings.Select(x => x.CreatedAt))
                    .Concat(model.BucketLists.Select(x => x.CreatedAt))
                    .Concat(model.Quotes.Select(x => x.CreatedAt))
                    .Concat(model.Journals.Select(x => x.CreatedAt))
                    .ToList();

                if (allDates.Count > 0)
                {
                    model.Start = allDates.Min().Date;
                    model.End = allDates.Max().Date;
                }
            }

            model.WorkoutsCount = model.StrengthTrainings.Count + model.EnduranceTrainings.Count;
            model.OpenTodosCount = model.Todos.Count(t => !t.IsCompleted);
            model.CompletedTodosCount = model.Todos.Count(t => t.IsCompleted);
            model.ActiveGoalsCount = model.Goals.Count(g => !g.IsCompleted);
            model.CompletedGoalsCount = model.Goals.Count(g => g.IsCompleted);
            model.TotalEntries = model.Weights.Count + model.Todos.Count + model.Goals.Count + model.Habits.Count + model.StrengthTrainings.Count + model.EnduranceTrainings.Count + model.BucketLists.Count + model.Quotes.Count + model.Journals.Count;

            model.RangeLabel = model.Granularity switch
            {
                SummaryGranularity.Day => model.Start.ToString("dddd, dd MMMM yyyy"),
                SummaryGranularity.Week => $"Week of {model.Start:yyyy-MM-dd}",
                SummaryGranularity.Month => model.Start.ToString("MMMM yyyy"),
                SummaryGranularity.Year => model.Start.ToString("yyyy"),
                SummaryGranularity.All => model.TotalEntries == 0 ? "All time" : $"All time · {model.Start:yyyy-MM-dd} to {model.End:yyyy-MM-dd}",
                _ => $"{model.Start:yyyy-MM-dd} to {model.End:yyyy-MM-dd}"
            };

            return View(model);
        }

        public async Task<IActionResult> Calendar(DateTime? date)
        {
            var model = new CalendarViewModel();

            var forDay = date != null ? date.Value.Date : DateTime.Now.Date;
            model.Date = forDay;

            var journals = await _context.Journals.OrderByDescending(o => o.CreatedAt).Take(5).ToListAsync();
            var activities = new List<CalendarViewModelEvent>();
            model.Events = activities;

            var allHabits = await _context.Habits.Where(t => !t.IsCompleted && t.RecurrenceRules != null && t.RecurrenceRules != "" && t.StartDate != null && t.EndDate != null).ToListAsync();
            var habits = calculateHabits(allHabits, forDay, forDay.AddDays(1).Date.AddTicks(-1));
            model.Events.AddRange(habits.ConvertAll(h => new CalendarViewModelEvent { Type = EntityType.Habit, Text = h.Name, StartDate = h.StartDate, EndDate = h.EndDate }).OrderBy(s => s.StartDate));

            // Add Due date for todos on forDay (Set start date and end date to due date to show as block)
            var todos = await _context.Todos.Where(t => !t.IsCompleted && t.DueDate != null && t.DueDate.Value.Date == forDay).ToListAsync();
            model.Events.AddRange(todos.ConvertAll(t => new CalendarViewModelEvent { Type = EntityType.Todo, Text = t.Title, StartDate = t.DueDate.Value, EndDate = t.DueDate.Value }));

            // Add End date for goals on forDay (Set start date to end date to show as block)
            var goals = await _context.Goals.Where(t => !t.IsCompleted && t.EndDate != null && t.EndDate.Value.Date == forDay).ToListAsync();
            model.Events.AddRange(goals.ConvertAll(g => new CalendarViewModelEvent { Type = EntityType.Goal, Text = g.Name, StartDate = g.EndDate.Value, EndDate = g.EndDate.Value }));

            return View(model);
        }

        public IActionResult Config()
        {
            var config = Models.Entities.Config.GetConfig(_context);
            var model = _mapper.Map<EditConfigViewModel>(config);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Config(long id, EditConfigViewModel configViewModel)
        {
            // Will always create the first entry if it doesn't exist which most likely was done in the view
            var configDb = Models.Entities.Config.GetConfig(_context);
            if (ModelState.IsValid)
            {
                configDb = _mapper.Map(configViewModel, configDb);
                configDb.SetUpdateFields();
                _context.Update(configDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Config), configViewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static List<Habit> calculateHabits(List<Habit> allHabits, DateTime startDate, DateTime endDate)
        {
            var habitList = new List<Habit>();
            foreach (var habit in allHabits)
            {
                var calendar = new Ical.Net.Calendar();
                calendar.Events.Add(new Ical.Net.CalendarComponents.CalendarEvent
                {
                    DtStart = new CalDateTime(DateTime.SpecifyKind(habit.StartDate.Value, DateTimeKind.Unspecified)),
                    DtEnd = new CalDateTime(DateTime.SpecifyKind(habit.EndDate.Value, DateTimeKind.Unspecified)),
                    Summary = habit.Name,
                    Description = habit.Description,
                    // Ical.Net 5: RecurrencePattern constructor no longer accepts the "RRULE:" prefix
                    RecurrenceRules = new List<RecurrencePattern> { new RecurrencePattern(RecurrenceRuleHelper.Normalize(habit.RecurrenceRules)) }
                });
                var calStartDate = new CalDateTime(DateTime.SpecifyKind(startDate, DateTimeKind.Unspecified));
                var calEndDate = new CalDateTime(DateTime.SpecifyKind(endDate, DateTimeKind.Unspecified));
                // Ical.Net 5: GetOccurrences(CalDateTime?, EvaluationOptions?) — filter end date manually
                calendar.GetOccurrences(calStartDate)
                    .TakeWhile(o => o.Period.StartTime <= calEndDate)
                    .ToList().ForEach(o =>
                {
                    habitList.Add(new Habit
                    {
                        Name = habit.Name,
                        Description = habit.Description,
                        StartDate = o.Period.StartTime.Value,
                        EndDate = (o.Period.EndTime ?? o.Period.StartTime).Value, // Use StartTime if EndTime is null (for all-day events)
                        RecurrenceRules = habit.RecurrenceRules,
                        IsCompleted = habit.IsCompleted
                    });
                });
            }

            return habitList.OrderBy(o => o.StartDate).Take(10).ToList();
        }
    }
}
