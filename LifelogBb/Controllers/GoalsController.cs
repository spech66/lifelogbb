﻿using Microsoft.AspNetCore.Mvc;
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
            var defaultSortOrder = $"{nameof(Goal.CreatedAt)}_desc";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == nameof(Goal.CreatedAt) ? defaultSortOrder : nameof(Goal.CreatedAt);

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
            goals = goals.SortByName(sortOrder, defaultSortOrder);

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
            this.AddCategoriesToViewData(_context);
            this.AddTagsToViewData(_context);
            return View();
        }

        // POST: Goals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted,Category,Tags")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                goal.SetCreateFields();
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
        public async Task<IActionResult> Edit(long id, [Bind("Name,Description,TargetValue,CurrentValue,StartDate,EndDate,IsCompleted,Category,Tags,Id")] EditGoalViewModel goalViewModel)
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
