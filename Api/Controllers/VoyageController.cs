using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("voyages")]
public class VoyageController(IVoyageService voyageService, DataContext context, IS3Service s3Service) : BaseApiController
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
    // Retrieve the user ID from token claims.
    var voyagerUserId = GetUserIdFromTokenClaims();
    
    // Return 401 if the User's ID can't be found.
    if (voyagerUserId == null)
    {
        return Unauthorized("User ID not found in token claims.");
    }
    
    try
    {
        // Save the voyage (and its stops) using your VoyageService.
        // Assume AddVoyageAsync returns the created voyage object (with an Id).
        var voyage = await voyageService.AddVoyageAsync(createVoyageModel, voyagerUserId.Value);

        // Generate pre-signed URLs for voyage images.
        var voyageUploadUrls = new List<string>();
        for (var i = 0; i < createVoyageModel.ImageCount; i++)
        {
            // Generate a unique key for each voyage image.
            var objectKey = $"voyages/{voyage.Id}/images/image-{Guid.NewGuid()}.jpg";
            var url = s3Service.GeneratePreSignedUrl(objectKey, TimeSpan.FromMinutes(15));
            voyageUploadUrls.Add(url);
        }

        // Generate pre-signed URLs for each stop's images.
        // Assuming each stop in createVoyageModel.Stops contains an ImageCount property.
        var stopsUploadUrls = new Dictionary<int, List<string>>();
        for (var i = 0; i < createVoyageModel.Stops.Count; i++)
        {
            var stop = createVoyageModel.Stops[i];
            var stopUrls = new List<string>();

            for (var j = 0; j < stop.ImageCount; j++)
            {
                // Create a unique key for each stop image.
                var objectKey = $"voyages/{voyage.Id}/stops/{i}/image-{Guid.NewGuid()}.jpg";
                var url = s3Service.GeneratePreSignedUrl(objectKey, TimeSpan.FromMinutes(1));
                stopUrls.Add(url);
            }

            stopsUploadUrls.Add(i, stopUrls);
        }

        // Return the voyage ID along with the generated upload URLs.
        return Ok(new 
        { 
            voyageId = voyage.Id, 
            voyageUploadUrls, 
            stopsUploadUrls 
        });
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