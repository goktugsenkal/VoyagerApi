using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("feed")]
[ApiController]
public class FeedController(IFeedService feedService) : BaseApiController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedList<VoyageDto>>> GetFeed(int pageNumber = 1, int pageSize = 10)
    {
        // return 401 if User's ID can't be found
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in token claims.");
        }
        
        if (pageNumber < 1 || pageSize < 1 || pageSize > 20)
        {
            return BadRequest("Page number and page size must be greater than 1 and page size must be less than or equal to 20.");
        }
        
        var voyages = await feedService.GetFeedAsync(pageNumber, pageSize, voyagerUserId.Value); // value is checked for null, safe
        return Ok(voyages); 
    }
}