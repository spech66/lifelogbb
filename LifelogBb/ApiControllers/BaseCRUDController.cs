using Microsoft.AspNetCore.Mvc;
using LifelogBb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LifelogBb.ApiDTOs;
using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseCRUDController<S, INP, OUTP> : ControllerBase where S : IBaseCRUDService<INP, OUTP> where OUTP : IBaseOutput
    {
        private readonly S _service;

        public BaseCRUDController(S service)
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

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult<OUTP>> Create(INP model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            try
            {
                var newEntitie = await _service.Create(model);
                return CreatedAtAction(nameof(Create), new { id = newEntitie.Id }, newEntitie);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OUTP>> Update(long id, INP model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            try
            {
                return Ok(await _service.Update(id, model));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteOutput>> Delete(long id)
        {
            var entitieId = await _service.Delete(id);

            if (entitieId == null)
                return NotFound();

            return Ok(new DeleteOutput() { Id = entitieId.Value });
        }

    }
}
