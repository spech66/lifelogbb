using Microsoft.AspNetCore.Mvc;
using LifelogBb.Models;
using LifelogBb.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LifelogBb.ApiServices
{
    public class BaseRService<TEntity, OUTP> : IBaseRService<OUTP> where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public BaseRService(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<ActionResult<IEnumerable<OUTP>>> GetAll()
        {
            var entities = await _repository.Query.ToListAsync();
            return _mapper.Map<List<OUTP>>(entities);
        }

        public virtual async Task<OUTP> GetById(long id)
        {
            var entity = await _repository.Query.FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<OUTP>(entity);
        }
    }
}
