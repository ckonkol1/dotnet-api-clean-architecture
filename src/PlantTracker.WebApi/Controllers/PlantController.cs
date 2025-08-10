using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.WebApi.Controllers;

/// <summary>
/// Plant Controller
/// </summary>
/// <param name="logger"></param>
/// <param name="plantService"></param>
[ApiController]
[ApiVersion(1)]
[Route("/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class PlantController(ILogger<PlantController> logger, IPlantService plantService) : ControllerBase
{
    /// <summary>
    /// Get All Plants
    /// </summary>
    /// <returns>A list of all the Plants</returns>
    [EndpointSummary("Get All Plants")]
    [EndpointDescription("This endpoint returns all plants stored in the database")]
    [EndpointName("GetAllPlants")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpGet(Name = "GetAllPlants")]
    public async Task<ActionResult<IEnumerable<PlantResponseModel>>> GetAllPlants()
    {
        List<PlantResponseModel> result = [.. (await plantService.GetAllPlantsAsync())];

        if (result?.Count == 0)
        {
            return NotFound("No plants were found.");
        }

        return Ok(result);
    }
}