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

        // A journal carries the day it is about, which can differ from the day it was written.
        // Matches the sort order of the journal list in the UI.
        protected override string DefaultSortOrder => $"{nameof(Journal.Date)}_desc";
    }
}
