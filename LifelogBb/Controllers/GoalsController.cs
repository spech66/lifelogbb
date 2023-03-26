using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Goals;
using LifelogBb.Utilities;

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
            var goals = from s in _context.Goals select s;

            int pageSize = 20;
            return View(await PaginatedList<Goal>.CreateAsync(goals.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            return View();
        }

        // POST: Goals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                goal.SetCreateFields();
                _context.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View(goal);
        }

        // POST: Goals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Description,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted,Id")] EditGoalViewModel goalViewModel)
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
                return Problem("Entity set 'LifelogBbContext.Goals'  is null.");
            }
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalExists(long id)
        {
          return (_context.Goals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
