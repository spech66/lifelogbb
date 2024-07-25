using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Goals;
using LifelogBb.Utilities;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Ical.Net;
using Ical.Net.DataTypes;

namespace LifelogBb.Controllers
{
    public class GoalsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public GoalsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Goals
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

            var goals = from s in _context.Goals select s;
            goals = goals.SortByName(sortOrder, $"{nameof(Goal.CreatedAt)}_desc");

            var config = Config.GetConfig(_context);
            ViewData["FeedToken"] = config.FeedToken;
            var list = await PaginatedList<Goal>.CreateAsync(goals.AsNoTracking(), pageNumber ?? 1, config.GoalPageSize);
            return View(new PaginatedListViewModel<Goal>(list, config));
        }

        // GET: Goals/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Goals == null)
            {
                return NotFound();
            }

            var goal = await _context.Goals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // GET: Goals/Create
        public IActionResult Create()
        {
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View();
        }

        // POST: Goals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,InitialValue,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted,Category,Tags")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                goal.SetCreateFields();
                if (goal.EndDate == null && goal.IsCompleted)
                {
                    goal.EndDate = DateTime.Now;
                }
                _context.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(goal);
        }

        // GET: Goals/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Goals == null)
            {
                return NotFound();
            }

            var goalDb = await _context.Goals.FindAsync(id);
            if (goalDb == null)
            {
                return NotFound();
            }

            var goal = _mapper.Map<EditGoalViewModel>(goalDb);
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View(goal);
        }

        // POST: Goals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Description,InitialValue,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted,Category,Tags,Id")] EditGoalViewModel goalViewModel)
        {
            if (id != goalViewModel.Id)
            {
                return NotFound();
            }

            var goalDb = await _context.Goals.FindAsync(id);
            if (ModelState.IsValid && goalDb != null)
            {
                try
                {
                    goalDb = _mapper.Map(goalViewModel, goalDb);
                    goalDb.SetUpdateFields();
                    _context.Update(goalDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoalExists(goalViewModel.Id))
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
            return View(goalViewModel);
        }

        // GET: Goals/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Goals == null)
            {
                return NotFound();
            }

            var goal = await _context.Goals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Goals == null)
            {
                return Problem("Entity set 'LifelogBbContext.Goals' is null.");
            }
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
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

            var goalsQuery = from s in _context.Goals select s;
            var goals = await goalsQuery.ToListAsync();
            goals.ToList().ForEach(goal =>
            {
                var percentage = 0;
                if (goal.TargetValue.HasValue && goal.CurrentValue.HasValue && goal.InitialValue.HasValue)
                {
                    var diffTarget = goal.TargetValue < goal.InitialValue ? goal.InitialValue.Value - goal.TargetValue.Value : goal.TargetValue.Value - goal.InitialValue.Value;
                    var diffCurrent = goal.TargetValue < goal.InitialValue ? goal.InitialValue.Value - goal.CurrentValue.Value : goal.CurrentValue.Value - goal.InitialValue.Value;
                    percentage = Convert.ToInt32(Math.Round((diffCurrent / diffTarget) * 100));
                }

                calendar.Todos.Add(new Ical.Net.CalendarComponents.Todo()
                {
                    Uid = goal.Id.ToString(),
                    Url = new Uri(Url.Action(nameof(Details), nameof(GoalsController).Replace("Controller", ""), new { id = goal.Id }, "https", Request.Host.Value)),
                    Summary = goal.Name,
                    Description = $"{goal.Description}\n\nInitial Value: {goal.InitialValue}\nCurrent Value: {goal.CurrentValue}\nTarget Value: {goal.TargetValue}\n",
                    Completed = goal.EndDate.HasValue ? new CalDateTime(goal.EndDate.Value) : null,
                    Start = goal.StartDate.HasValue ? new CalDateTime(goal.StartDate.Value) : null,
                    Priority = 0, // habit.IsImportant ? 1 : 5, // 0-9, 0=undefined, 1=highest, 9=lowest
                    Status = goal.IsCompleted ? "COMPLETED" : (percentage > 0 ? "IN-PROCESS" : ""),
                    Categories = new List<string>() { goal.Category ?? "" },
                    PercentComplete = percentage, // 0 = not started, 1=100
                });
            });

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            return Results.Content(serializedCalendar, contentType: "text/calendar");
        }

        private bool GoalExists(long id)
        {
          return (_context.Goals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
