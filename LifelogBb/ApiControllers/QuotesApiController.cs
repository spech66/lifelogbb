using LifelogBb.ApiDTOs.Quotes;
using LifelogBb.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/quotes")]
    [ApiController]
    public class QuotesApiController : BaseCRUDController<QuotesService, QuoteInput, QuoteOutput>
    {
        public QuotesApiController(QuotesService service) : base(service)
        {
        }
    }
}
