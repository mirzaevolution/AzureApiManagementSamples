using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureApiManagementSamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuidController : ControllerBase
    {
        [HttpGet("")]
        [ProducesResponseType(200)]
        public IActionResult Get() => Ok(new
        {
            data = new string[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            }
        });
    }
}