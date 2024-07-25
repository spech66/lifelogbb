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
                    DtStart = new CalDateTime(habit.StartDate.Value),
                    DtEnd = new CalDateTime(habit.EndDate.Value),
                    Summary = habit.Name,
                    Description = habit.Description,
                    RecurrenceRules = new List<RecurrencePattern> { new RecurrencePattern(habit.RecurrenceRules) }
                });
                calendar.GetOccurrences(startDate, endDate).ToList().ForEach(o =>
                {
                    habitList.Add(new Habit
                    {
                        Name = habit.Name,
                        Description = habit.Description,
                        StartDate = o.Period.StartTime.Value,
                        EndDate = o.Period.EndTime.Value,
                        RecurrenceRules = habit.RecurrenceRules,
                        IsCompleted = habit.IsCompleted
                    });
                });
            }

            return habitList.OrderBy(o => o.StartDate).Take(10).ToList();
        }
    }
}
