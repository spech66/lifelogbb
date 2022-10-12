using AutoMapper;
using LifelogBb.ApiDTOs.Journals;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class JournalsService : BaseCRUDService<Journal, JournalInput, JournalOutput>
    {
        public JournalsService(IRepository<Journal> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
