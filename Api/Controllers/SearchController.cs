using Core.Constants;
using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("search")]
public class SearchController(ISearchService searchService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await searchService.GlobalSearchAsync(query);
        return Ok(results);
    }
}