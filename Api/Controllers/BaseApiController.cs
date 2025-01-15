using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class BaseApiController : ControllerBase
{
    protected Guid? GetUserIdFromTokenClaims()
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
    }
}