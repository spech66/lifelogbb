using Microsoft.AspNetCore.Mvc;
using LifelogBb.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LifelogBb.Models.Entities;

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
            //var notes = _repository.Query.GetPaged(1, 3);
            //return _mapper.Map<PagedResult<NoteOutput>>(notes);
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
