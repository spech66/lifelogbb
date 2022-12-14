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
using LifelogBb.Models.Habits;

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
        public async Task<IActionResult> Index()
        {
              return _context.Habits != null ? 
                          View(await _context.Habits.OrderByDescending(o => o.CreatedAt).ToListAsync()) :
                          Problem("Entity set 'LifelogBbContext.Habits'  is null.");
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
