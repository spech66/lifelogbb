using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Interfaces
{
    public interface IBaseCRUDService<INP, OUTP>
    {
        public Task<ActionResult<IEnumerable<OUTP>>> GetAll();
        public Task<OUTP> GetById(long id);
        public Task<OUTP> Create(INP inputModel);
        public Task<OUTP> Update(long id, INP inputModel);
        public Task<long?> Delete(long id);
    }
}
