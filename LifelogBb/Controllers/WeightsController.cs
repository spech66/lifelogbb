using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using LifelogBb.Models.Weights;
using AutoMapper;
using LifelogBb.Utilities;

namespace LifelogBb.Controllers
{
    public class WeightsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public WeightsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Weights
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

            var weights = from s in _context.Weights select s;
            weights = weights.FilterByDoubleProps(nameof(Weight.BodyWeight), searchString, 1.0);
            weights = weights.SortByName(sortOrder, $"{nameof(Weight.CreatedAt)}_desc");

            var config = Config.GetConfig(_context);
            int pageSize = config.WeightPageSize;
            ViewData["UnitsType"] = config.UnitsType;
            return View(await PaginatedList<Weight>.CreateAsync(weights.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Weights/Graph
        public IActionResult Graph()
        {
            return View();
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

            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weight);
        }

        // GET: Weights/Create
        public IActionResult Create()
        {
            Weight? weight = _context.Weights.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
            if(weight != null)
            {
                weight.Id = 0;
                ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
                return View(weight);
            }

            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View();
        }

        // POST: Weights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Height,BodyWeight")] Weight weight)
        {
            if (ModelState.IsValid)
            {
                CalculateBmi(weight);
                weight.SetCreateFields();
                _context.Add(weight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weight);
        }

        // GET: Weights/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Weights == null)
            {
                return NotFound();
            }

            var weightDb = await _context.Weights.FindAsync(id);
            if (weightDb == null)
            {
                return NotFound();
            }
            var weight = _mapper.Map<EditWeightViewModel>(weightDb);
            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weight);
        }

        // POST: Weights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Height,BodyWeight,Id")] EditWeightViewModel weightViewModel)
        {
            if (id != weightViewModel.Id)
            {
                return NotFound();
            }

            var weightDb = await _context.Weights.FindAsync(id);
            if (ModelState.IsValid && weightDb != null)
            {
                try
                {
                    weightDb = _mapper.Map(weightViewModel, weightDb);
                    CalculateBmi(weightDb);
                    weightDb.SetUpdateFields();
                    _context.Update(weightDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeightExists(weightViewModel.Id))
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
            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weightViewModel);
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

            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weight);
        }

        // POST: Weights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Weights == null)
            {
                return Problem("Entity set 'LifelogBbContext.Weights' is null.");
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

        private void CalculateBmi(Weight weight)
        {
            var measurements = Config.GetConfig(_context).UnitsType;
            if (measurements == Measurements.Metric)
            {
                weight.Bmi = (weight.BodyWeight * 1.0) / (((weight.Height * 0.01) * weight.Height) * 0.01);
            } else
            {
                weight.Bmi = weight.BodyWeight / (weight.Height * weight.Height) * 703.0;
            }
        }
    }
}
