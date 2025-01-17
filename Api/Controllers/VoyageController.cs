using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("voyages")]
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
    
    /// <summary>
    /// Get all voyages
    /// </summary>
    /// <returns>List of Voyage</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ICollection<VoyageDto>>> GetAllVoyagesAsync
        ([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in token claims.");
        }
        
        return Ok(await voyageService.GetAllVoyagesAsync(pageNumber, pageSize));
    }
    
    /// <summary>
    /// Get a specific voyage by its Id
    /// </summary>
    /// <param name="voyageId"></param>
    /// <returns>Voyage</returns>
    [HttpGet("{voyageId:guid}")]
    [Authorize]
    public async Task<ActionResult<VoyageDto>> GetVoyage(Guid voyageId)
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in token claims.");
        }
        
        var voyage = await voyageService.GetVoyageByIdAsync(voyageId);
        
        if (voyage == null)
        {
            return NotFound("Voyage not found.");
        }
        
        return voyage;
    }
    
    /// <summary>
    /// Update a specific voyage
    /// </summary>
    /// <param name="voyageId"></param>
    /// <param name="updateVoyageModel"></param>
    /// <returns>Voyage</returns>
    [HttpPut("{voyageId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateVoyage(Guid voyageId, UpdateVoyageModel updateVoyageModel)
    {
        var voyage = await voyageService.GetVoyageByIdAsync(voyageId);
        
        if (voyage == null)
        {
            return NotFound("Voyage not found.");
        }
        
        await voyageService.UpdateVoyageAsync(voyageId, updateVoyageModel);
        
        return Ok("Voyage updated successfully.");
    }
    
    /// <summary>
    /// Delete a specific voyage
    /// </summary>
    /// <param name="voyageId"></param>
    /// <returns></returns>
    [HttpDelete("{voyageId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteVoyage(Guid voyageId)
    {
        var voyage = await voyageService.GetVoyageByIdAsync(voyageId);
        
        if (voyage == null)
        {
            return NotFound("Voyage not found.");
        }
        
        await voyageService.DeleteVoyageAsync(voyageId);
        
        return Ok("Voyage deleted successfully.");
    }
    
    [HttpGet("error")]
    [Authorize]
    // todo: remove
    public IActionResult Error()
    {
        throw new Exception("This is an unhandled exception.");
    }
}