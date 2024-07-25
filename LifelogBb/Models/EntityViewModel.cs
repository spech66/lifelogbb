using LifelogBb.Models.Entities;

namespace LifelogBb.Models
{
    public class EntityViewModel<T> where T : BaseEntity
    {
        public T Entity { get; set; }

        public Config Config { get; set; }

        public EntityViewModel(T entity, Config config)
        {
            Entity = entity;
            Config = config;
        }
    }
}
