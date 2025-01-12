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
    /// <summary>
    /// Endpoint that creates a plan or a voyage, based on {"isComplete": true}
    /// </summary>
    /// <param name="createVoyageModel"></param>
    /// <returns>Voyage</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Voyage>> CreateVoyage(CreateVoyageModel createVoyageModel)
    {
        // extract the user ID from the claims
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        // return 401 if userId not found in token
        if (userIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        // try parsing userId to a Guid, return 400 bad request if parsing can't be done 
        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return BadRequest("Invalid User ID format.");
        }
        
        // create a voyage with the request model
        var voyage = new Voyage
        {
            Title = createVoyageModel.Title,
            Description = createVoyageModel.Description,
            LocationName = createVoyageModel.LocationName,
            StartDate = createVoyageModel.StartDate,
            EndDate = createVoyageModel.EndDate,
            StopCount = createVoyageModel.StopCount,
            Currency = createVoyageModel.Currency,
            ExpectedPrice = createVoyageModel.ExpectedPrice,
            VoyagerUserId = userId
        };
        
        // pass voyage to the service layer to be added to the database, shouldn't fail.
        // error management will solve giving clear responses in case the request fails
        // at lower layers.
        await voyageRepository.CreateVoyageAsync(voyage);
        return Ok(voyage);
    }

}