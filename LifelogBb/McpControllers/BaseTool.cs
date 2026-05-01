using LifelogBb.Interfaces;
using LifelogBb.Interfaces.DTOs;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;

namespace LifelogBb.McpControllers
{
    [McpServerToolType]
    public class BaseTool<S, INP, OUTP>
    {
        protected readonly S _service;

        public BaseTool(S service)
        {
            _service = service;
        }
    }
}
