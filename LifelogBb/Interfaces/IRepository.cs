using LifelogBb.Models;

namespace LifelogBb.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public LifelogBbContext Context { get; }

        IQueryable<TEntity> Query { get; }

        Task<TEntity?> Find(params object[] keys);

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}
