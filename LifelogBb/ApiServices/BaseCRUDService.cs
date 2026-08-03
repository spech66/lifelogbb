using Microsoft.AspNetCore.Mvc;
using LifelogBb.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LifelogBb.Models.Entities;
using LifelogBb.Utilities;

namespace LifelogBb.ApiServices
{
    public class BaseCRUDService<TEntity, INP, OUTP> : IBaseCRUDService<INP, OUTP> where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public BaseCRUDService(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<ActionResult<IEnumerable<OUTP>>> GetAll()
        {
            var entities = await _repository.Query.ToListAsync();
            return _mapper.Map<List<OUTP>>(entities);
        }

        /// <summary>
        /// Sort order applied when the caller does not ask for one. Entities with their own date
        /// field override this so that "newest" means the same here as it does in the UI.
        /// </summary>
        protected virtual string DefaultSortOrder => $"{nameof(BaseEntity.CreatedAt)}_desc";

        /// <summary>
        /// Filters, sorts and limits in one query. Results are always ordered so that a limit is
        /// deterministic; without an explicit sortOrder the newest entries come first.
        /// </summary>
        public virtual async Task<ActionResult<IEnumerable<OUTP>>> GetAll(string? filterJson, string? sortOrder = null, int? limit = null)
        {
            EnsureSortableField(sortOrder);

            if (limit is < 1)
                throw new ArgumentException($"Limit must be at least 1 but was {limit}.");

            IQueryable<TEntity> query = _repository.Query
                .FilterByGroup<TEntity>(filterJson, throwOnInvalidFilter: true)
                .SortByName(sortOrder ?? string.Empty, DefaultSortOrder);

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var entities = await query.ToListAsync();
            return _mapper.Map<List<OUTP>>(entities);
        }

        /// <summary>
        /// SortByName silently falls back to its default for unknown fields. Callers of the API and
        /// the MCP tools get a clear error instead, so a typo does not look like a successful sort.
        /// Checked against the EF model rather than the CLR type, because computed properties such
        /// as Weight.BmiOverweight are not mapped to a column and would only fail once EF tries to
        /// translate the query.
        /// </summary>
        private void EnsureSortableField(string? sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
                return;

            var field = sortOrder.EndsWith("_desc") ? sortOrder[..^5] : sortOrder;
            var entityType = _repository.Context.Model.FindEntityType(typeof(TEntity));
            if (entityType?.FindProperty(field) == null)
                throw new ArgumentException($"Unknown sort field '{field}'.");
        }

        public virtual async Task<OUTP> GetById(long id)
        {
            var entitie = await _repository.Query.FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<OUTP>(entitie);
        }

        public virtual async Task<OUTP> Create(INP inputModel)
        {
            var entry = _mapper.Map<TEntity>(inputModel);
            entry.SetCreateFields();

            _repository.Insert(entry);
            await _repository.Context.SaveChangesAsync();

            return _mapper.Map<OUTP>(entry);
        }

        public virtual async Task<OUTP> Update(long id, INP inputModel)
        {
            var dbEntry = await _repository.Query.FirstOrDefaultAsync(m => m.Id == id);

            if (dbEntry == null || id != dbEntry.Id)
                throw new Exception("Invalid id");

            dbEntry = _mapper.Map<INP, TEntity>(inputModel, dbEntry);
            dbEntry.SetUpdateFields();

            _repository.Update(dbEntry);
            await _repository.Context.SaveChangesAsync();
            return _mapper.Map<OUTP>(dbEntry);
        }

        public virtual async Task<long?> Delete(long id)
        {
            var dbEntry = await _repository.Query.FirstOrDefaultAsync(m => m.Id == id);

            if (dbEntry == null)
                return null;

            _repository.Delete(dbEntry);
            await _repository.Context.SaveChangesAsync();

            return id;
        }

        /*private bool ItemExists(long id, long userId)
        {
            return _repository.Query.Any(m => m.UserId == userId && m.Id == id);
        }*/
    }
}
