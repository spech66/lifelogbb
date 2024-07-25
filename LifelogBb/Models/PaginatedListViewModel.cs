using LifelogBb.Models.Entities;
using LifelogBb.Utilities;

namespace LifelogBb.Models
{
    public class PaginatedListViewModel<T> where T : BaseEntity
    {
        public PaginatedList<T> List { get; set; }

        public Config Config { get; set; }

        public PaginatedListViewModel(PaginatedList<T> list, Config config)
        {
            List = list;
            Config = config;
        }
    }
}
