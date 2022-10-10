using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Weight;

namespace LifelogBb.Controllers
{
    public class WeightsController : Controller
    {
        private readonly LifelogBbContext _context;

        public WeightsController(LifelogBbContext context)
        {
            _context = context;
        }

        // GET: Weights
        public async Task<IActionResult> Index()
        {
              return View(await _context.Weights.ToListAsync());
        }

        // GET: Weights/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Weights == null)
            {
                return NotFound();
            }

            var weight = await _context.Weights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weight == null)
            {
                return NotFound();
            }

            return View(weight);
        }

        // GET: Weights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Weights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Height,BodyWeight")] Weight weight)
        {
            if (ModelState.IsValid)
            {
                weight.Bmi = ((weight.BodyWeight * 1.0M) / (((weight.Height * 0.01M) * weight.Height) * 0.01M));
                weight.CreatedAt = weight.UpdatedAt = DateTime.Now;
                _context.Add(weight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weight);
        }

        // GET: Weights/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Weights == null)
            {
                return NotFound();
            }

            var weight = await _context.Weights.FindAsync(id);
            if (weight == null)
            {
                return NotFound();
            }
            return View(weight);
        }

        // POST: Weights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Height,BodyWeight,Id")] Weight weight)
        {
            if (id != weight.Id)
            {
                return NotFound();
            }

            var weightDb = await _context.Weights.FindAsync(id);
            if (ModelState.IsValid && weightDb != null)
            {
                try
                {
                    weightDb.Height = weight.Height;
                    weightDb.BodyWeight = weight.BodyWeight;
                    weightDb.Bmi = ((weight.BodyWeight * 1.0M) / (((weight.Height * 0.01M) * weight.Height) * 0.01M));
                    weightDb.UpdatedAt = DateTime.Now;
                    _context.Update(weightDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeightExists(weight.Id))
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
            return View(weight);
        }

        // GET: Weights/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Weights == null)
            {
                return NotFound();
            }

            var weight = await _context.Weights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weight == null)
            {
                return NotFound();
            }

            return View(weight);
        }

        // POST: Weights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Weights == null)
            {
                return Problem("Entity set 'LifelogBbContext.Weights'  is null.");
            }
            var weight = await _context.Weights.FindAsync(id);
            if (weight != null)
            {
                _context.Weights.Remove(weight);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeightExists(long id)
        {
          return _context.Weights.Any(e => e.Id == id);
        }
    }
}
