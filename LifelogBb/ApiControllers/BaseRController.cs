using Microsoft.AspNetCore.Mvc;
using LifelogBb.Interfaces;
using LifelogBb.Interfaces.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LifelogBb.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseRController<S, OUTP> : ControllerBase where S : IBaseRService<OUTP> where OUTP : IBaseOutput
    {
        protected readonly S _service;

        public BaseRController(S service)
        {
            _service = service;
        }

        // GET: api/[controller]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<OUTP>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OUTP>> GetById(long id)
        {
            var note = await _service.GetById(id);

            if (note == null)
                return NotFound();

            return note;
        }
    }
}
