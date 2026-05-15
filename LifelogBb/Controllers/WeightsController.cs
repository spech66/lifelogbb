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
        public async Task<IActionResult> Index()
        {
            var config = Config.GetConfig(_context);

            var latest = await _context.Weights.OrderByDescending(o => o.CreatedAt).FirstOrDefaultAsync();
            var previous = await _context.Weights.OrderByDescending(o => o.CreatedAt).Skip(1).FirstOrDefaultAsync();

            double? change30Days = null;
            if (latest != null)
            {
                var cutoff30 = latest.CreatedAt.AddDays(-30);
                var entry30 = await _context.Weights
                    .Where(w => w.CreatedAt <= cutoff30)
                    .OrderByDescending(w => w.CreatedAt)
                    .FirstOrDefaultAsync();
                if (entry30 != null)
                {
                    change30Days = Math.Round(latest.BodyWeight - entry30.BodyWeight, 1);
                }
            }

            var allTimeMin = await _context.Weights.MinAsync(w => (double?)w.BodyWeight);
            var allTimeMax = await _context.Weights.MaxAsync(w => (double?)w.BodyWeight);

            var recentEntries = await _context.Weights
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();

            var model = new WeightIndexViewModel
            {
                Latest = latest,
                Previous = previous,
                Change30Days = change30Days,
                AllTimeMin = allTimeMin,
                AllTimeMax = allTimeMax,
                RecentEntries = recentEntries,
                Config = config
            };

            return View(model);
        }

        // GET: Weights/Table
        public async Task<IActionResult> Table(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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
            var list = await PaginatedList<Weight>.CreateAsync(weights.AsNoTracking(), pageNumber ?? 1, config.WeightPageSize);
            return View(new PaginatedListViewModel<Weight>(list, config));
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
                var model = new EditWeightViewModel
                {
                    BodyWeight = weight.BodyWeight
                };
                ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
                return View(model);
            }

            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View();
        }

        // POST: Weights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BodyWeight")] EditWeightViewModel weightViewModel)
        {
            if (ModelState.IsValid)
            {
                var config = Config.GetConfig(_context);
                var weight = new Weight
                {
                    BodyWeight = weightViewModel.BodyWeight,
                    Height = config.Height
                };
                weight.Bmi = BmiHelper.Calculate(weight.BodyWeight, weight.Height, config.UnitsType);
                weight.SetCreateFields();
                _context.Add(weight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UnitsType"] = Config.GetConfig(_context).UnitsType;
            return View(weightViewModel);
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
        public async Task<IActionResult> Edit(long id, [Bind("BodyWeight,Id")] EditWeightViewModel weightViewModel)
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
                    var config = Config.GetConfig(_context);
                    weightDb.Height = config.Height;
                    weightDb.Bmi = BmiHelper.Calculate(weightDb.BodyWeight, weightDb.Height, config.UnitsType);
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
    }
}
