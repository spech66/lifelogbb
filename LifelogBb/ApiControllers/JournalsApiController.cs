using LifelogBb.ApiServices;
using LifelogBb.ApiDTOs.Journals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.ApiControllers
{
    [Route("api/journals")]
    [ApiController]
    public class JournalsApiController : BaseCRUDController<JournalsService, JournalInput, JournalOutput>
    {
        public JournalsApiController(JournalsService service) : base(service)
        {
        }
    }
}
