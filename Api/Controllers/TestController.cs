using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("version")]
public class TestController : BaseApiController
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("0.1.3);
    }
}