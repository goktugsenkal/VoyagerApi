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
        if (pageNumber < 1 || pageSize < 1 || pageSize > 20)
        {
            return BadRequest("Page number and page size must be greater than 1 and page size must be less than or equal to 20.");
        }
        
        var voyages = await feedService.GetFeedAsync(pageNumber, pageSize);
        return Ok(voyages); 
    }
}