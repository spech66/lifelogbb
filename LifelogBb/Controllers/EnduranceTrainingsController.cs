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
using LifelogBb.Models.StrengthTrainings;
using LifelogBb.Models.EnduranceTrainings;

namespace LifelogBb.Controllers
{
    public class EnduranceTrainingsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public EnduranceTrainingsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: EnduranceTrainings
        public async Task<IActionResult> Index()
        {
              return View(await _context.EnduranceTrainings.OrderByDescending(o => o.CreatedAt).ToListAsync());
        }

        // GET: EnduranceTrainings/Graph
        public async Task<IActionResult> Graph(string? exercise)
        {
            var enduranceTrainings = from et in _context.EnduranceTrainings select et;

            if (!String.IsNullOrEmpty(exercise))
            {
                enduranceTrainings = enduranceTrainings.Where(s => s.Exercise == exercise);
            }

            return View(await enduranceTrainings.OrderBy(o => o.CreatedAt).ToListAsync());
        }

        // GET: EnduranceTrainings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.EnduranceTrainings == null)
            {
                return NotFound();
            }

            var enduranceTraining = await _context.EnduranceTrainings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enduranceTraining == null)
            {
                return NotFound();
            }

            return View(enduranceTraining);
        }

        // GET: EnduranceTrainings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EnduranceTrainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Exercise,Distance,Duration,Notes,Rating")] EnduranceTraining enduranceTraining)
        {
            if (ModelState.IsValid)
            {
                enduranceTraining.CreatedAt = enduranceTraining.UpdatedAt = DateTime.Now;
                _context.Add(enduranceTraining);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enduranceTraining);
        }

        // GET: EnduranceTrainings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.EnduranceTrainings == null)
            {
                return NotFound();
            }

            var enduranceTrainingDb = await _context.EnduranceTrainings.FindAsync(id);
            if (enduranceTrainingDb == null)
            {
                return NotFound();
            }
            var enduranceTraining = _mapper.Map<EditEnduranceTrainingViewModel>(enduranceTrainingDb);
            return View(enduranceTraining);
        }

        // POST: EnduranceTrainings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Exercise,Distance,Duration,Notes,Rating,Id")] EditEnduranceTrainingViewModel enduranceTrainingViewModel)
        {
            if (id != enduranceTrainingViewModel.Id)
            {
                return NotFound();
            }

            var enduranceTrainingDb = await _context.EnduranceTrainings.FindAsync(id);
            if (ModelState.IsValid && enduranceTrainingDb != null)
            {
                try
                {
                    enduranceTrainingDb = _mapper.Map(enduranceTrainingViewModel, enduranceTrainingDb);
                    enduranceTrainingDb.UpdatedAt = DateTime.Now;
                    _context.Update(enduranceTrainingDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnduranceTrainingExists(enduranceTrainingViewModel.Id))
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
            return View(enduranceTrainingViewModel);
        }

        // GET: EnduranceTrainings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.EnduranceTrainings == null)
            {
                return NotFound();
            }

            var enduranceTraining = await _context.EnduranceTrainings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enduranceTraining == null)
            {
                return NotFound();
            }

            return View(enduranceTraining);
        }

        // POST: EnduranceTrainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.EnduranceTrainings == null)
            {
                return Problem("Entity set 'LifelogBbContext.EnduranceTrainings'  is null.");
            }
            var enduranceTraining = await _context.EnduranceTrainings.FindAsync(id);
            if (enduranceTraining != null)
            {
                _context.EnduranceTrainings.Remove(enduranceTraining);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnduranceTrainingExists(long id)
        {
          return _context.EnduranceTrainings.Any(e => e.Id == id);
        }
    }
}
