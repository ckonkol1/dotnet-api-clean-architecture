using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using System.Net;

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
    [EndpointSummary("Get All Plants")]
    [EndpointDescription("This endpoint returns all plants stored in the database")]
    [EndpointName("GetAllPlants")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpGet(Name = "GetAllPlants")]
    public async Task<ActionResult<IEnumerable<PlantResponseModel>>> GetAllPlants()
    {
        var result = new List<PlantResponseModel>();
        var errorResponse = new ErrorResponse();
        try
        {
            result = [.. (await plantService.GetAllPlantsAsync())];

            if (result?.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error Occurred");
            errorResponse.ErrorMessages.Add(ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, result);
        }
    }
}