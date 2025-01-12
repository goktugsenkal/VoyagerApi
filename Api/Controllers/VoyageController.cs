using System.Security.Claims;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("voyage")]
public class VoyageController(IVoyageService voyageService) : ControllerBase
{
    /// <summary>
    /// Endpoint that creates a plan or a voyage, based on {"isComplete": true}
    /// </summary>
    /// <param name="createVoyageModel"></param>
    /// <returns>Voyage</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateVoyage(CreateVoyageModel createVoyageModel)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        // return 401 if User's ID can't be found
        if (voyagerUserIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        // parse User's ID into a Guid
        var voyagerUserId = Guid.Parse(voyagerUserIdClaim.Value);
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            await voyageService.AddVoyageAsync(createVoyageModel, voyagerUserId);
            
            return Ok("Voyage created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}