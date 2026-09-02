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
        public async Task<IActionResult> Index()
        {
            var all = await _context.StrengthTrainings
                .OrderByDescending(s => s.Date).ThenByDescending(s => s.CreatedAt)
                .ToListAsync();

            var personalRecords = all
                .GroupBy(s => s.Exercise)
                .Select(g => new StrengthTrainingPersonalRecord
                {
                    Exercise = g.Key,
                    // Max/Sum over a nullable weight skips the sets that have none instead of
                    // treating bodyweight work as a 0 kg record.
                    MaxWeight = g.Max(s => s.Weight),
                    MaxReps = g.Max(s => s.Reps),
                    MaxVolume = g.Max(s => TrainingSetFormat.Volume(s.Reps, s.Weight)),
                    TotalSessions = g.Count()
                })
                .OrderBy(r => r.Exercise)
                .ToList();

            var model = new StrengthTrainingIndexViewModel
            {
                TotalSessions = all.Count,
                UniqueExerciseCount = personalRecords.Count,
                TotalVolume = all.Sum(s => TrainingSetFormat.Volume(s.Reps, s.Weight)) ?? 0,
                LastSession = all.FirstOrDefault(),
                PersonalRecords = personalRecords,
                RecentSessions = all.Take(5).ToList()
            };

            return View(model);
        }

        // GET: StrengthTrainings/Table
        public async Task<IActionResult> Table(string sortOrder, string currentFilter, string searchString, string? filter, int? pageNumber)
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
            ViewData["Filter"] = filter;

            var trainings = from s in _context.StrengthTrainings select s;
            trainings = trainings.FilterByGroup(filter);

            // The chosen column alone leaves ties -- a whole training day ties on Date -- and an unordered
            // tie is free to shuffle rows between pages. Logging order breaks the tie, in the same
            // direction as the chosen column: the default descending table is a log of every set with the
            // most recent on top, so a second plan trained later the same day sorts above the first one.
            var defaultSort = $"{nameof(StrengthTraining.Date)}_desc";
            var byColumn = trainings.SortByName(sortOrder, defaultSort);
            var sorted = trainings.ResolveSortOrder(sortOrder, defaultSort).EndsWith("_desc")
                ? byColumn.ThenByDescending(s => s.CreatedAt).ThenByDescending(s => s.Id)
                : byColumn.ThenBy(s => s.CreatedAt).ThenBy(s => s.Id);

            var config = Config.GetConfig(_context);
            var list = await PaginatedList<StrengthTraining>.CreateAsync(sorted.Include(s => s.TrainingPlan).AsNoTracking(), pageNumber ?? 1, config.StrengthTrainingPageSize);
            return View(new PaginatedListViewModel<StrengthTraining>(list, config));
        }

        // GET: StrengthTrainings/Graph
        public async Task<IActionResult> Graph(string? exercise)
        {
            var exercises = await _context.StrengthTrainings.Select(s => s.Exercise).Distinct().OrderBy(e => e).ToListAsync();
            var model = new StrengthTrainingGraphViewModel
            {
                Exercises = exercises,
                SelectedExercise = !string.IsNullOrEmpty(exercise) && exercises.Contains(exercise) ? exercise : null
            };
            return View("Graph", model);
        }

        // Kept for backward compatibility with existing links; unused by the current Graph view.
        public async Task<IActionResult> GraphGet(string? exercise)
        {
            var strengthTrainings = from st in _context.StrengthTrainings select st;

            if (!String.IsNullOrEmpty(exercise))
            {
                strengthTrainings = strengthTrainings.Where(s => s.Exercise == exercise);
            }

            return Json(await strengthTrainings.OrderBy(o => o.Date).ToListAsync());
        }

        // GET: StrengthTrainings/GraphExerciseData
        // One aggregated point per training day instead of one point per set -- a set-level bar chart
        // over the whole history is unreadable once more than a handful of sessions exist.
        public async Task<IActionResult> GraphExerciseData(string exercise, DateTime? from)
        {
            var query = _context.StrengthTrainings.Where(s => s.Exercise == exercise);
            if (from.HasValue)
            {
                query = query.Where(s => s.Date >= from.Value.Date);
            }

            var sets = await query.ToListAsync();

            var days = sets
                .GroupBy(s => s.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    topWeight = g.Max(s => s.Weight),
                    est1Rm = g.Max(s => s.Weight * (1 + s.Reps / 30.0)) is double oneRm ? Math.Round(oneRm, 1) : (double?)null,
                    volume = g.Sum(s => TrainingSetFormat.Volume(s.Reps, s.Weight)) ?? 0,
                    sets = g.Count(),
                    totalReps = g.Sum(s => s.Reps)
                })
                .ToList();

            return Json(new { exercise, days });
        }

        // GET: StrengthTrainings/GraphOverviewData
        public async Task<IActionResult> GraphOverviewData(DateTime? from)
        {
            var query = _context.StrengthTrainings.AsQueryable();
            if (from.HasValue)
            {
                query = query.Where(s => s.Date >= from.Value.Date);
            }

            var sets = await query.ToListAsync();
            var startOfWeek = Config.GetConfig(_context).StartOfWeek;

            DateTime WeekStart(DateTime date)
            {
                var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
                return date.Date.AddDays(-diff);
            }

            var days = sets
                .GroupBy(s => s.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    volume = g.Sum(s => TrainingSetFormat.Volume(s.Reps, s.Weight)) ?? 0,
                    sets = g.Count(),
                    exercises = g.Select(s => s.Exercise).Distinct().Count()
                })
                .ToList();

            var weeks = sets
                .GroupBy(s => WeekStart(s.Date))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    weekStart = g.Key.ToString("yyyy-MM-dd"),
                    volume = g.Sum(s => TrainingSetFormat.Volume(s.Reps, s.Weight)) ?? 0,
                    trainingDays = g.Select(s => s.Date.Date).Distinct().Count(),
                    sets = g.Count()
                })
                .ToList();

            return Json(new { days, weeks });
        }

        // GET: StrengthTrainings/Sessions
        public async Task<IActionResult> Sessions(DateTime? from)
        {
            var query = _context.StrengthTrainings.AsQueryable();
            if (from.HasValue)
            {
                query = query.Where(s => s.Date >= from.Value.Date);
            }

            var sets = await query.Include(s => s.TrainingPlan).ToListAsync();

            // One lookup instead of a count query per session row.
            var plannedSetCounts = await _context.TrainingPlanSets
                .GroupBy(ps => ps.TrainingPlanId)
                .Select(g => new { PlanId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.PlanId, g => g.Count);

            // Grouped by day *and* plan: two plans worked on the same day are two sessions, and sets logged
            // without a plan stay their own row instead of being folded into whichever plan came first.
            var sessions = sets
                .GroupBy(s => new { Date = s.Date.Date, s.TrainingPlanId })
                .OrderByDescending(g => g.Key.Date)
                .ThenBy(g => g.Min(s => s.CreatedAt))
                .Select(g => new StrengthTrainingSession
                {
                    Date = g.Key.Date,
                    SetCount = g.Count(),
                    Volume = g.Sum(s => TrainingSetFormat.Volume(s.Reps, s.Weight)) ?? 0,
                    Exercises = g.Select(s => s.Exercise).Distinct().OrderBy(e => e).ToList(),
                    TrainingPlanId = g.Key.TrainingPlanId,
                    TrainingPlanName = g.Select(s => s.TrainingPlan).FirstOrDefault(p => p != null)?.Name,
                    PlannedSetCount = g.Key.TrainingPlanId is long planId && plannedSetCounts.TryGetValue(planId, out var plannedCount)
                        ? plannedCount
                        : (int?)null
                })
                .ToList();

            return View(sessions);
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
        public async Task<IActionResult> Create()
        {
            var exercises = await _context.StrengthTrainings.Select(s => s.Exercise).Distinct().ToListAsync();
            ViewData["ExerciseList"] = string.Join(",", exercises);
            return View(new StrengthTraining { Date = DateTime.UtcNow.Date });
        }

        // POST: StrengthTrainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Exercise,Reps,Weight,DurationSeconds,Notes,Rating,Date")] StrengthTraining strengthTraining)
        {
            if (strengthTraining.Date == default)
            {
                strengthTraining.Date = DateTime.UtcNow.Date;
            }

            if (ModelState.IsValid)
            {
                strengthTraining.SetCreateFields();
                _context.Add(strengthTraining);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var exercises = await _context.StrengthTrainings.Select(s => s.Exercise).Distinct().ToListAsync();
            ViewData["ExerciseList"] = string.Join(",", exercises);
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
        public async Task<IActionResult> Edit(long id, [Bind("Exercise,Reps,Weight,DurationSeconds,Notes,Rating,Date,Id")] EditStrengthTrainingViewModel strengthTrainingViewModel)
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
                return Problem("Entity set 'LifelogBbContext.StrengthTrainings' is null.");
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
