using AutoMapper;
using LifelogBb.ApiDTOs.Quotes;
using LifelogBb.Interfaces;
using LifelogBb.Models.Entities;

namespace LifelogBb.ApiServices
{
    public class QuotesService : BaseCRUDService<Quote, QuoteInput, QuoteOutput>
    {
        public QuotesService(IRepository<Quote> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
