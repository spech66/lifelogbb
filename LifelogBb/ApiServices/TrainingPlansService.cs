using AutoMapper;
using LifelogBb.ApiDTOs.TrainingPlans;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.ApiServices
{
    // The generic BaseCRUDService/EntityRepository stack does no .Include(), so a plan's Sets would
    // never be loaded. This service overrides every read/write operation to load and manage the Sets
    // child collection explicitly, while still implementing IBaseCRUDService so BaseCRUDController and
    // BaseTool (API + MCP) work unmodified.
    public class TrainingPlansService : BaseCRUDService<TrainingPlan, TrainingPlanInput, TrainingPlanOutput>
    {
        public TrainingPlansService(IRepository<TrainingPlan> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string DefaultSortOrder => $"{nameof(TrainingPlan.CreatedAt)}_desc";

        public override async Task<ActionResult<IEnumerable<TrainingPlanOutput>>> GetAll()
        {
            var entities = await _repository.Query.Include(p => p.Sets).ToListAsync();
            return _mapper.Map<List<TrainingPlanOutput>>(entities);
        }

        public override async Task<ActionResult<IEnumerable<TrainingPlanOutput>>> GetAll(string? filterJson, string? sortOrder = null, int? limit = null)
        {
            sortOrder = string.IsNullOrWhiteSpace(sortOrder) ? DefaultSortOrder : sortOrder;

            if (limit is < 1)
                throw new ArgumentException($"Limit must be at least 1 but was {limit}.");

            var query = _repository.Query
                .Include(p => p.Sets)
                .FilterByGroup<TrainingPlan>(filterJson, throwOnInvalidFilter: true)
                .SortByName(sortOrder, DefaultSortOrder);

            IQueryable<TrainingPlan> limited = query;
            if (limit.HasValue)
                limited = limited.Take(limit.Value);

            var entities = await limited.ToListAsync();
            return _mapper.Map<List<TrainingPlanOutput>>(entities);
        }

        public override async Task<TrainingPlanOutput> GetById(long id)
        {
            var entity = await _repository.Query.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<TrainingPlanOutput>(entity);
        }

        public override async Task<TrainingPlanOutput> Create(TrainingPlanInput inputModel)
        {
            var plan = _mapper.Map<TrainingPlan>(inputModel);
            plan.SetCreateFields();
            plan.Sets = BuildSets(inputModel.Sets, plan);

            _repository.Insert(plan);
            await _repository.Context.SaveChangesAsync();

            return _mapper.Map<TrainingPlanOutput>(plan);
        }

        public override async Task<TrainingPlanOutput> Update(long id, TrainingPlanInput inputModel)
        {
            var plan = await _repository.Query.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                throw new Exception("Invalid id");

            _mapper.Map(inputModel, plan);
            plan.SetUpdateFields();

            // Full replace: any StrengthTraining rows that pointed at a removed set are cleared to
            // null via the SetNull FK behavior configured in LifelogBbContext, not deleted.
            _repository.Context.TrainingPlanSets.RemoveRange(plan.Sets);
            plan.Sets = BuildSets(inputModel.Sets, plan);

            await _repository.Context.SaveChangesAsync();
            return _mapper.Map<TrainingPlanOutput>(plan);
        }

        public async Task<TrainingPlanOutput?> CopyPlan(long sourceId, DateTime? date, string? name)
        {
            var source = await _repository.Query.Include(p => p.Sets).FirstOrDefaultAsync(p => p.Id == sourceId);
            if (source == null)
                return null;

            var copy = new TrainingPlan
            {
                Name = string.IsNullOrWhiteSpace(name) ? $"{source.Name} (Copy)" : name,
                Description = source.Description,
                Date = date?.Date,
                IsArchived = false
            };
            copy.SetCreateFields();

            copy.Sets = source.Sets
                .OrderBy(s => s.SortOrder)
                .Select((s, index) =>
                {
                    var set = new TrainingPlanSet
                    {
                        Exercise = s.Exercise,
                        SortOrder = index,
                        Reps = s.Reps,
                        Weight = s.Weight,
                        Notes = s.Notes,
                        TrainingPlan = copy
                    };
                    set.SetCreateFields();
                    return set;
                })
                .ToList();

            _repository.Insert(copy);
            await _repository.Context.SaveChangesAsync();

            return _mapper.Map<TrainingPlanOutput>(copy);
        }

        private static List<TrainingPlanSet> BuildSets(List<TrainingPlanSetInput> inputs, TrainingPlan plan)
        {
            var sets = new List<TrainingPlanSet>();
            for (var index = 0; index < inputs.Count; index++)
            {
                var input = inputs[index];
                var set = new TrainingPlanSet
                {
                    Exercise = input.Exercise ?? string.Empty,
                    SortOrder = index,
                    Reps = input.Reps,
                    Weight = input.Weight,
                    Notes = input.Notes,
                    TrainingPlan = plan
                };
                set.SetCreateFields();
                sets.Add(set);
            }
            return sets;
        }
    }
}
