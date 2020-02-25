using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NatMarchand.YayNay.ApiApp.Controllers
{
    [ApiController]
    [Route("my")]
    public class MyControllers : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}