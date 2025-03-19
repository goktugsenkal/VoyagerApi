using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("version")]
public class TestController(DataContext context) : BaseApiController
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("0.2.1");
    }
}