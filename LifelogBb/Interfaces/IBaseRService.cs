using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Interfaces
{
    public interface IBaseRService<OUTP>
    {
        public Task<ActionResult<IEnumerable<OUTP>>> GetAll();
        public Task<ActionResult<IEnumerable<OUTP>>> GetAll(string? filterJson);
        public Task<OUTP> GetById(long id);
    }
}
