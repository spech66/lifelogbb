using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Habits;
using LifelogBb.Utilities;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using Ical.Net.Serialization;
using Ical.Net.CalendarComponents;
using Ical.Net;
using Ical.Net.DataTypes;

namespace LifelogBb.Controllers
{
    public class HabitsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public HabitsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Habits
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var habits = from s in _context.Habits select s;
            habits = habits.SortByName(sortOrder, $"{nameof(Habit.CreatedAt)}_desc");

            var config = Config.GetConfig(_context);
            ViewData["FeedToken"] = config.FeedToken;
            var list = await PaginatedList<Habit>.CreateAsync(habits.AsNoTracking(), pageNumber ?? 1, config.HabitPageSize);
            return View(new PaginatedListViewModel<Habit>(list, config));
        }

        // GET: Habits/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Habits == null)
            {
                return NotFound();
            }

            var habit = await _context.Habits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // GET: Habits/Create
        public IActionResult Create()
        {
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View();
        }

        // POST: Habits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,RecurrenceRules,IsCompleted,Category,Tags")] Habit habit)
        {
            if (ModelState.IsValid)
            {
                habit.SetCreateFields();
                _context.Add(habit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(habit);
        }

        // GET: Habits/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Habits == null)
            {
                return NotFound();
            }

            var habitDb = await _context.Habits.FindAsync(id);
            if (habitDb == null)
            {
                return NotFound();
            }

            var habit = _mapper.Map<EditHabitViewModel>(habitDb);
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(habit);
        }

        // POST: Habits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Description,StartDate,EndDate,RecurrenceRules,IsCompleted,Category,Tags,Id")] EditHabitViewModel habitViewModel)
        {
            if (id != habitViewModel.Id)
            {
                return NotFound();
            }

            var habitDb = await _context.Habits.FindAsync(id);
            if (ModelState.IsValid && habitDb != null)
            {
                try
                {
                    habitDb = _mapper.Map(habitViewModel, habitDb);
                    habitDb.SetUpdateFields();
                    _context.Update(habitDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitExists(habitViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(habitViewModel);
        }

        // GET: Habits/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Habits == null)
            {
                return NotFound();
            }

            var habit = await _context.Habits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // POST: Habits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Habits == null)
            {
                return Problem("Entity set 'LifelogBbContext.Habits' is null.");
            }
            var habit = await _context.Habits.FindAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IResult> Feed(string token)
        {
            var config = Config.GetConfig(_context);
            if (config == null || config.FeedToken == null || config.FeedToken != token)
            {
                return Results.Content("Unauthorized", contentType: "text/plain");
            }

            var calendar = new Calendar();
            calendar.AddTimeZone(new VTimeZone(config.FeedTimeZone));

            var habitsQuery = from s in _context.Habits select s;
            var habits = await habitsQuery.ToListAsync();
            habits.ToList().ForEach(habit =>
            {
                if(!habit.StartDate.HasValue)
                {
                    return;
                }

                var calEvent = new CalendarEvent()
                {
                    Uid = habit.Id.ToString(),
                    Url = new Uri(Url.Action(nameof(Details), nameof(HabitsController).Replace("Controller", ""), new { id = habit.Id }, "https", Request.Host.Value)),
                    Summary = habit.Name,
                    Description = habit.Description,
                    LastModified = new CalDateTime(habit.UpdatedAt),
                    Start = habit.StartDate.HasValue ? new CalDateTime(habit.StartDate.Value) : null,
                    End = habit.EndDate.HasValue ? new CalDateTime(habit.EndDate.Value) : null,
                    IsAllDay = !habit.EndDate.HasValue,
                };

                if (habit.RecurrenceRules != null)
                {
                    RecurrencePattern recurrenceRule = new RecurrencePattern(habit.RecurrenceRules);
                    calEvent.RecurrenceRules = new List<RecurrencePattern>() { recurrenceRule };
                }

                calendar.Events.Add(calEvent);
            });

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            return Results.Content(serializedCalendar, contentType: "text/calendar");
        }

        private bool HabitExists(long id)
        {
          return (_context.Habits?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
