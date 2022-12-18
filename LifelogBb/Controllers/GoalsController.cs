using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Quotes;
using LifelogBb.Models.Goals;

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
        public async Task<IActionResult> Index()
        {
              return _context.Goals != null ? 
                          View(await _context.Goals.OrderByDescending(o => o.CreatedAt).ToListAsync()) :
                          Problem("Entity set 'LifelogBbContext.Goals'  is null.");
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
