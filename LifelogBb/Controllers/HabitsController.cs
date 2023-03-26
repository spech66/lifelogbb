﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.Habits;
using LifelogBb.Utilities;

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
            var defaultSortOrder = $"{nameof(Habit.CreatedAt)}_desc";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == nameof(Habit.CreatedAt) ? defaultSortOrder : nameof(Habit.CreatedAt);

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
            habits = habits.SortByName(sortOrder, defaultSortOrder);

            int pageSize = 20;
            return View(await PaginatedList<Habit>.CreateAsync(habits.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            return View();
        }

        // POST: Habits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Frequency,FrequencyUnit,StartDate,EndDate,ExtraRules,IsCompleted")] Habit habit)
        {
            if (ModelState.IsValid)
            {
                habit.SetCreateFields();
                _context.Add(habit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View(habit);
        }

        // POST: Habits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Description,Frequency,FrequencyUnit,StartDate,EndDate,ExtraRules,IsCompleted,Id")] EditHabitViewModel habitViewModel)
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
                return Problem("Entity set 'LifelogBbContext.Habits'  is null.");
            }
            var habit = await _context.Habits.FindAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitExists(long id)
        {
          return (_context.Habits?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
