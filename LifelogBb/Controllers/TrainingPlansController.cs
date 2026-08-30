using System.Text.Json;
using AutoMapper;
using LifelogBb.ApiServices;
using LifelogBb.Models;
using LifelogBb.Models.Entities;
using LifelogBb.Models.TrainingPlans;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Controllers
{
    public class TrainingPlansController : Controller
    {
        private readonly LifelogBbContext _context;
        private readonly TrainingPlansService _service;
        protected readonly IMapper _mapper;

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        // plansetseditor.js reads plain lowercase field names (exercise/reps/weight/durationSeconds/notes)
        // from the hidden SetsJson input it seeds itself with, so server-rendered JSON must match that casing.
        private static readonly JsonSerializerOptions CamelCaseJsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public TrainingPlansController(LifelogBbContext context, TrainingPlansService service, IMapper mapper)
        {
            _context = context;
            _service = service;
            _mapper = mapper;
        }

        // GET: TrainingPlans
        public async Task<IActionResult> Index()
        {
            var all = await _context.TrainingPlans.Include(p => p.Sets).ToListAsync();

            var model = new TrainingPlanIndexViewModel
            {
                Templates = all.Where(p => p.Date == null && !p.IsArchived).OrderBy(p => p.Name).ToList(),
                DayPlans = all.Where(p => p.Date != null && !p.IsArchived).OrderByDescending(p => p.Date).ToList(),
                Archived = all.Where(p => p.IsArchived).OrderByDescending(p => p.UpdatedAt).ToList()
            };

            return View(model);
        }

        // GET: TrainingPlans/Table
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

            var plans = _context.TrainingPlans.Include(p => p.Sets).AsQueryable();
            plans = plans.FilterByGroup(filter);
            plans = plans.SortByName(sortOrder, $"{nameof(TrainingPlan.CreatedAt)}_desc");

            var config = Config.GetConfig(_context);
            var list = await PaginatedList<TrainingPlan>.CreateAsync(plans.AsNoTracking(), pageNumber ?? 1, config.TrainingPlanPageSize);
            return View(new PaginatedListViewModel<TrainingPlan>(list, config));
        }

        // GET: TrainingPlans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.TrainingPlans.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            var model = await BuildComparisonAsync<TrainingPlanDetailsViewModel>(plan);
            return View(model);
        }

        // GET: TrainingPlans/Create
        public async Task<IActionResult> Create()
        {
            await PopulateExerciseListAsync();
            return View(new EditTrainingPlanViewModel { SetsJson = "[]" });
        }

        // POST: TrainingPlans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Date,IsArchived,SetsJson")] EditTrainingPlanViewModel model)
        {
            var rows = ParseSetsJson(model.SetsJson);
            ValidateRows(rows);

            if (ModelState.IsValid)
            {
                var plan = new TrainingPlan
                {
                    Name = model.Name,
                    Description = model.Description,
                    Date = model.Date,
                    IsArchived = model.IsArchived
                };
                plan.SetCreateFields();
                plan.Sets = BuildSets(rows, plan);

                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateExerciseListAsync();
            return View(model);
        }

        // GET: TrainingPlans/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.TrainingPlans.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            var model = _mapper.Map<EditTrainingPlanViewModel>(plan);
            model.SetsJson = JsonSerializer.Serialize(plan.Sets.OrderBy(s => s.SortOrder)
                .Select(s => new PlanSetRow { Exercise = s.Exercise, Reps = s.Reps, Weight = s.Weight, DurationSeconds = s.DurationSeconds, Notes = s.Notes }),
                CamelCaseJsonOptions);

            await PopulateExerciseListAsync();
            return View(model);
        }

        // POST: TrainingPlans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Date,IsArchived,SetsJson")] EditTrainingPlanViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            var rows = ParseSetsJson(model.SetsJson);
            ValidateRows(rows);

            var plan = await _context.TrainingPlans.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                plan.Name = model.Name;
                plan.Description = model.Description;
                plan.Date = model.Date;
                plan.IsArchived = model.IsArchived;
                plan.SetUpdateFields();

                // Full replace. StrengthTraining rows linked to a removed set keep their history; only
                // the link is cleared (SetNull FK behavior configured in LifelogBbContext).
                _context.TrainingPlanSets.RemoveRange(plan.Sets);
                plan.Sets = BuildSets(rows, plan);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateExerciseListAsync();
            return View(model);
        }

        // GET: TrainingPlans/Copy/5
        public async Task<IActionResult> Copy(long? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.TrainingPlans.FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            ViewData["SourceName"] = plan.Name;
            return View(new CopyTrainingPlanViewModel { SourceId = plan.Id, Name = $"{plan.Name} (Copy)" });
        }

        // POST: TrainingPlans/Copy/5
        [HttpPost, ActionName("Copy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CopyConfirmed(CopyTrainingPlanViewModel model)
        {
            var result = await _service.CopyPlan(model.SourceId, model.Date, model.Name);
            if (result == null)
                return NotFound();

            return RedirectToAction(nameof(Edit), new { id = result.Id });
        }

        // GET: TrainingPlans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.TrainingPlans.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            return View(plan);
        }

        // POST: TrainingPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var plan = await _context.TrainingPlans.FindAsync(id);
            if (plan != null)
            {
                _context.TrainingPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingPlans/Workout/5
        public async Task<IActionResult> Workout(long? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.TrainingPlans.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound();

            // A template is reusable and has no single day of its own -- starting a workout from it
            // creates a dated copy for today so the workout instance and its done-state are unambiguous.
            if (plan.Date == null)
            {
                var copy = await _service.CopyPlan(plan.Id, DateTime.UtcNow.Date, plan.Name);
                if (copy == null)
                    return NotFound();

                return RedirectToAction(nameof(Workout), new { id = copy.Id });
            }

            var model = await BuildComparisonAsync<WorkoutViewModel>(plan);
            return View(model);
        }

        public class ConfirmSetRequest
        {
            public long PlanSetId { get; set; }
            public int Reps { get; set; }
            public double? Weight { get; set; }
            public int? DurationSeconds { get; set; }
            public int Rating { get; set; } = 3;
            public string? Notes { get; set; }
        }

        // POST: TrainingPlans/WorkoutConfirmSet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WorkoutConfirmSet([FromBody] ConfirmSetRequest request)
        {
            var planSet = await _context.TrainingPlanSets.Include(s => s.TrainingPlan).FirstOrDefaultAsync(s => s.Id == request.PlanSetId);
            if (planSet == null)
                return NotFound();

            // What arrives here is whatever the client posted, so the same contract the plan editor and
            // the API enforce is applied before anything is written.
            if (request.Reps < 0)
                return BadRequest(new { ok = false, error = "Reps cannot be negative." });
            if (request.Weight < 0)
                return BadRequest(new { ok = false, error = "Weight cannot be negative." });
            if (request.DurationSeconds is not null && (request.DurationSeconds < 1 || request.DurationSeconds > TrainingSetRules.MaxDurationSeconds))
                return BadRequest(new { ok = false, error = $"Duration must be between 1 and {TrainingSetRules.MaxDurationSeconds} seconds." });
            if (!TrainingSetRules.HasEffort(request.Reps, request.DurationSeconds))
                return BadRequest(new { ok = false, error = "A set needs either reps or a duration." });

            var training = new StrengthTraining
            {
                Exercise = planSet.Exercise,
                Reps = request.Reps,
                Weight = request.Weight,
                DurationSeconds = request.DurationSeconds,
                Rating = request.Rating,
                Notes = request.Notes,
                Date = planSet.TrainingPlan.Date ?? DateTime.UtcNow.Date,
                TrainingPlanId = planSet.TrainingPlanId,
                TrainingPlanSetId = planSet.Id
            };
            training.SetCreateFields();

            _context.StrengthTrainings.Add(training);
            await _context.SaveChangesAsync();

            return Json(new { ok = true, trainingId = training.Id });
        }

        public class UndoSetRequest
        {
            public long TrainingId { get; set; }
        }

        // POST: TrainingPlans/WorkoutUndoSet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WorkoutUndoSet([FromBody] UndoSetRequest request)
        {
            var training = await _context.StrengthTrainings.FirstOrDefaultAsync(t => t.Id == request.TrainingId);
            if (training == null || training.TrainingPlanSetId == null)
                return NotFound();

            _context.StrengthTrainings.Remove(training);
            await _context.SaveChangesAsync();

            return Json(new { ok = true });
        }

        private async Task<T> BuildComparisonAsync<T>(TrainingPlan plan) where T : new()
        {
            var trainings = await _context.StrengthTrainings
                .Where(t => t.TrainingPlanId == plan.Id)
                .ToListAsync();

            var byPlanSet = trainings
                .Where(t => t.TrainingPlanSetId != null)
                .GroupBy(t => t.TrainingPlanSetId!.Value)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.CreatedAt).First());

            var rows = plan.Sets.OrderBy(s => s.SortOrder)
                .Select(s => new PlanSetComparisonRow
                {
                    Planned = s,
                    Actual = byPlanSet.TryGetValue(s.Id, out var actual) ? actual : null
                })
                .ToList();

            var extras = trainings.Where(t => t.TrainingPlanSetId == null).OrderBy(t => t.CreatedAt).ToList();

            object model = typeof(T) == typeof(WorkoutViewModel)
                ? new WorkoutViewModel { Plan = plan, Rows = rows, ExtraTrainings = extras }
                : new TrainingPlanDetailsViewModel { Plan = plan, Rows = rows, ExtraTrainings = extras };

            return (T)model;
        }

        private static List<PlanSetRow> ParseSetsJson(string? setsJson)
        {
            if (string.IsNullOrWhiteSpace(setsJson))
                return new List<PlanSetRow>();

            try
            {
                return JsonSerializer.Deserialize<List<PlanSetRow>>(setsJson, JsonOptions) ?? new List<PlanSetRow>();
            }
            catch (JsonException)
            {
                return new List<PlanSetRow>();
            }
        }

        private void ValidateRows(List<PlanSetRow> rows)
        {
            if (rows.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one set.");
                return;
            }

            for (var i = 0; i < rows.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(rows[i].Exercise))
                    ModelState.AddModelError(string.Empty, $"Set {i + 1}: exercise is required.");
                if (rows[i].Reps < 0)
                    ModelState.AddModelError(string.Empty, $"Set {i + 1}: reps cannot be negative.");
                if (rows[i].Weight < 0)
                    ModelState.AddModelError(string.Empty, $"Set {i + 1}: weight cannot be negative.");
                if (rows[i].DurationSeconds is not null && (rows[i].DurationSeconds < 1 || rows[i].DurationSeconds > TrainingSetRules.MaxDurationSeconds))
                    ModelState.AddModelError(string.Empty, $"Set {i + 1}: duration must be between 1 and {TrainingSetRules.MaxDurationSeconds} seconds.");
                if (!TrainingSetRules.HasEffort(rows[i].Reps, rows[i].DurationSeconds))
                    ModelState.AddModelError(string.Empty, $"Set {i + 1}: needs either reps or a duration.");
            }
        }

        private static List<TrainingPlanSet> BuildSets(List<PlanSetRow> rows, TrainingPlan plan)
        {
            var sets = new List<TrainingPlanSet>();
            for (var i = 0; i < rows.Count; i++)
            {
                var set = new TrainingPlanSet
                {
                    Exercise = rows[i].Exercise,
                    SortOrder = i,
                    Reps = rows[i].Reps,
                    Weight = rows[i].Weight,
                    DurationSeconds = rows[i].DurationSeconds,
                    Notes = rows[i].Notes,
                    TrainingPlan = plan
                };
                set.SetCreateFields();
                sets.Add(set);
            }
            return sets;
        }

        private async Task PopulateExerciseListAsync()
        {
            var exercises = await _context.StrengthTrainings.Select(s => s.Exercise).Distinct().ToListAsync();
            ViewData["ExerciseList"] = string.Join(",", exercises);
        }
    }

    public class CopyTrainingPlanViewModel
    {
        public long SourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? Date { get; set; }
    }
}
