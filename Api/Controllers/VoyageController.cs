using System.Security.Claims;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("voyage")]
public class VoyageController(IVoyageRepository voyageRepository) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Voyage>> CreateVoyage(CreateVoyageModel request)
    {
        // Extract the user ID from the claims
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "userId");
    
        if (userIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }

        Guid userId;
        if (!Guid.TryParse(userIdClaim.Value, out userId))
        {
            return BadRequest("Invalid User ID format.");
        }

        var voyage = new Voyage
        {
            Title = request.Title,
            Description = request.Description,
            LocationName = request.LocationName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StopCount = request.StopCount,
            Currency = request.Currency,
            ExpectedPrice = request.ExpectedPrice,
            VoyagerUserId = userId
        };

        await voyageRepository.CreateVoyage(voyage);

        return Ok(voyage);
    }

}