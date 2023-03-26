using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using AutoMapper;
using LifelogBb.Models.StrengthTrainings;
using LifelogBb.Utilities;

namespace LifelogBb.Controllers
{
    public class StrengthTrainingsController : Controller
    {
        private readonly LifelogBbContext _context;
        protected readonly IMapper _mapper;

        public StrengthTrainingsController(LifelogBbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: StrengthTrainings
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var defaultSortOrder = $"{nameof(StrengthTraining.CreatedAt)}_desc";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == nameof(StrengthTraining.CreatedAt) ? defaultSortOrder : nameof(StrengthTraining.CreatedAt);

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var trainings = from s in _context.StrengthTrainings select s;
            trainings = trainings.SortByName(sortOrder, defaultSortOrder);

            int pageSize = 20;
            return View(await PaginatedList<StrengthTraining>.CreateAsync(trainings.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: StrengthTrainings/Graph
        public IActionResult Graph(string? exercise)
        {
            return View("Graph", exercise);
        }

        public async Task<IActionResult> GraphGet(string? exercise)
        {
            var strengthTrainings = from st in _context.StrengthTrainings select st;

            if (!String.IsNullOrEmpty(exercise))
            {
                strengthTrainings = strengthTrainings.Where(s => s.Exercise == exercise);
            }

            return Json(await strengthTrainings.OrderBy(o => o.CreatedAt).ToListAsync());
        }

        // GET: StrengthTrainings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.StrengthTrainings == null)
            {
                return NotFound();
            }

            var strengthTraining = await _context.StrengthTrainings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (strengthTraining == null)
            {
                return NotFound();
            }

            return View(strengthTraining);
        }

        // GET: StrengthTrainings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StrengthTrainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Exercise,Reps,Weight,Notes,Rating")] StrengthTraining strengthTraining)
        {
            if (ModelState.IsValid)
            {
                strengthTraining.SetCreateFields();
                _context.Add(strengthTraining);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(strengthTraining);
        }

        // GET: StrengthTrainings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.StrengthTrainings == null)
            {
                return NotFound();
            }

            var strengthTrainingDb = await _context.StrengthTrainings.FindAsync(id);
            if (strengthTrainingDb == null)
            {
                return NotFound();
            }
            var strengthTraining = _mapper.Map<EditStrengthTrainingViewModel>(strengthTrainingDb);
            return View(strengthTraining);
        }

        // POST: StrengthTrainings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Exercise,Reps,Weight,Notes,Rating,Id")] EditStrengthTrainingViewModel strengthTrainingViewModel)
        {
            if (id != strengthTrainingViewModel.Id)
            {
                return NotFound();
            }

            var strengthTrainingDb = await _context.StrengthTrainings.FindAsync(id);
            if (ModelState.IsValid && strengthTrainingDb != null)
            {
                try
                {
                    strengthTrainingDb = _mapper.Map(strengthTrainingViewModel, strengthTrainingDb);
                    strengthTrainingDb.SetUpdateFields();
                    _context.Update(strengthTrainingDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrengthTrainingExists(strengthTrainingViewModel.Id))
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
            return View(strengthTrainingViewModel);
        }

        // GET: StrengthTrainings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.StrengthTrainings == null)
            {
                return NotFound();
            }

            var strengthTraining = await _context.StrengthTrainings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (strengthTraining == null)
            {
                return NotFound();
            }

            return View(strengthTraining);
        }

        // POST: StrengthTrainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.StrengthTrainings == null)
            {
                return Problem("Entity set 'LifelogBbContext.StrengthTrainings'  is null.");
            }
            var strengthTraining = await _context.StrengthTrainings.FindAsync(id);
            if (strengthTraining != null)
            {
                _context.StrengthTrainings.Remove(strengthTraining);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrengthTrainingExists(long id)
        {
          return _context.StrengthTrainings.Any(e => e.Id == id);
        }
    }
}
