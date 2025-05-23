using Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("version")]
public class TestController : BaseApiController
{
    [HttpGet]
    public ActionResult<string> GetVersion()
    {
        return Content(VersionInfo.ApiVersion, "text/plain");
    }
}