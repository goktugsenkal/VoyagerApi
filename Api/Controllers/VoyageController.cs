using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("voyage")]
public class VoyageController(IVoyageService voyageService) : BaseApiController
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
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in token claims.");
        }
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            await voyageService.AddVoyageAsync(createVoyageModel, voyagerUserId.Value);
            // null case is checked, so it's okay to use voyagerUserId.Value
            
            return Ok("Voyage created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}