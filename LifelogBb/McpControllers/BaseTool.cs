using LifelogBb.Interfaces;
using LifelogBb.Interfaces.DTOs;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class BaseTool<S, INP, OUTP> where S : IBaseCRUDService<INP, OUTP>
    {
        protected readonly S _service;

        public BaseTool(S service)
        {
            _service = service;
        }

        protected async Task<IEnumerable<OUTP>> GetAllFiltered(string? filter)
        {
            var result = filter != null
                ? await _service.GetAll(filter)
                : await _service.GetAll();
            return result.Value ?? [];
        }
    }
}
