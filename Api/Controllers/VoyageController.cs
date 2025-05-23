using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Models.Voyage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("voyages")]
public class VoyageController(IVoyageService voyageService) : BaseApiController
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateVoyage(CreateVoyageModel createVoyageModel)
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in token claims.");
        }

        try
        {
            ValidateMediaTypes(createVoyageModel);
            var result = await voyageService.AddVoyageWithMediaAsync(createVoyageModel, voyagerUserId.Value);
            return Ok(new 
            {
                voyageId = result.Voyage.Id,
                voyageUploadUrls = result.VoyageUploadUrls,
                stopsUploadUrls = result.StopsUploadUrls
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    private static readonly HashSet<string> AllowedMediaTypes = new()
    {
        "mp4", "jpg", "jpeg", "png", "gif"
    };

    private void ValidateMediaTypes(CreateVoyageModel model)
    {
        foreach (var media in model.MediaTypes ?? [])
        {
            if (!AllowedMediaTypes.Contains(media.ToLower()))
                throw new InvalidOperationException($"Unsupported media type: {media}");
        }

        foreach (var stop in model.Stops ?? [])
        {
            foreach (var media in stop.MediaTypes ?? [])
            {
                if (!AllowedMediaTypes.Contains(media.ToLower()))
                    throw new InvalidOperationException($"Unsupported stop media type: {media}");
            }
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
    
    [HttpGet("map")]
    [Authorize]
    public ActionResult<IEnumerable<Voyage>> GetVoyagesFiltered(
        [FromQuery] double? latitudeMin,
        [FromQuery] double? latitudeMax,
        [FromQuery] double? longitudeMin,
        [FromQuery] double? longitudeMax,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Validate pagination parameters.
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Page number and page size must be greater than zero.");
        }

        // Validate coordinate bounds.
        if (latitudeMin.HasValue && latitudeMax.HasValue && latitudeMin.Value > latitudeMax.Value)
        {
            return BadRequest("latitudeMin must be less than or equal to latitudeMax.");
        }

        if (longitudeMin.HasValue && longitudeMax.HasValue && longitudeMin.Value > longitudeMax.Value)
        {
            return BadRequest("longitudeMin must be less than or equal to longitudeMax.");
        }

        // Validate that coordinates are within acceptable ranges.
        if (latitudeMin is < -90 or > 90)
        {
            return BadRequest("latitudeMin must be between -90 and 90.");
        }
        if (latitudeMax is < -90 or > 90)
        {
            return BadRequest("latitudeMax must be between -90 and 90.");
        }
        if (longitudeMin is < -180 or > 180)
        {
            return BadRequest("longitudeMin must be between -180 and 180.");
        }
        if (longitudeMax is < -180 or > 180)
        {
            return BadRequest("longitudeMax must be between -180 and 180.");
        }

        var voyages = voyageService.GetVoyagesFiltered(latitudeMin, latitudeMax, longitudeMin, longitudeMax, pageNumber, pageSize);
        return Ok(voyages);
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
        
        var voyage = await voyageService.GetVoyageDtoByIdAsync(voyageId, voyagerUserId.Value); // checked for null, safe
        
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
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        var voyage = await voyageService.GetVoyageByIdAsync(voyageId);
        
        if (voyage == null)
        {
            return NotFound("Voyage not found.");
        }
        
        if (voyagerUserId != voyage.VoyagerUserId)
        {
            return Unauthorized("You are not authorized to update this voyage.");    
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
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        var voyage = await voyageService.GetVoyageByIdAsync(voyageId);
        
        if (voyage == null)
        {
            return NotFound("Voyage not found.");
        }
        
        if (voyagerUserId != voyage.VoyagerUserId)
        {
            return Unauthorized("You are not authorized to delete this voyage.");    
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