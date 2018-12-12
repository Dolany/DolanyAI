namespace Dolany.Ai.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        [HttpPost]
        public IActionResult Req([FromBody]string msg)
        {
            Console.WriteLine(msg);

            return new JsonResult(new { res = 1 });
        }
    }
}