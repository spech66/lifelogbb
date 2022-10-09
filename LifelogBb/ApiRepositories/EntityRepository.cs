using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LifelogBb.Interfaces;
using System.Threading.Tasks;
using LifelogBb.Models;

namespace LifelogBb.ApiRepositories
{
    public class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly LifelogBbContext _context;
        private readonly DbSet<TEntity> entitiesSet;

        public EntityRepository(LifelogBbContext context)
        {
            _context = context;

            entitiesSet = context.Set<TEntity>();
        }

        public LifelogBbContext Context => _context;

        public IQueryable<TEntity> Query => entitiesSet;

        public async Task<TEntity?> Find(params object[] keys)
        {
            return await entitiesSet.FindAsync(keys);
        }

        public void Insert(TEntity entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
                entitiesSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
                entitiesSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
                entitiesSet.Attach(entity);
            entitiesSet.Remove(entity);
        }
    }
}
