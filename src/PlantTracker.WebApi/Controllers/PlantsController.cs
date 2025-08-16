using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.WebApi.Controllers;

/// <summary>
/// Plant Controller
/// </summary>
/// <param name="plantService"></param>
[ApiController]
[ApiVersion(1)]
[Route("/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class PlantsController(IPlantService plantService) : ControllerBase
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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
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

    /// <summary>
    /// Get All Plants
    /// </summary>
    /// <returns>A list of all the Plants</returns>
    [EndpointSummary("Get Plant by Id")]
    [EndpointDescription("This endpoint returns a plant with the provided Id")]
    [EndpointName("GetPlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [HttpGet("{id:guid}", Name = "GetPlantById")]
    public async Task<ActionResult<PlantResponseModel>> GetPlantById([FromRoute] Guid id)
    {
        var plant = await plantService.GetPlantByIdAsync(id);
        if (plant == null)
        {
            return NotFound($"Resource Not Found. Id: {id}");
        }

        return Ok();
    }

    /// <summary>
    /// Create Plant
    /// </summary>
    /// <returns>Id of Plant</returns>
    [EndpointSummary("Create Plant")]
    [EndpointDescription("Creates a Plant")]
    [EndpointName("CreatePlant")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [HttpPut(Name = "CreatePlant")]
    public async Task<ActionResult<string>> CreatePlant([FromBody] CreatePlantRequestModel createPlantRequestModel)
    {
        var id = await plantService.CreatePlantAsync(createPlantRequestModel.ToPlantModel());
        return Created(string.Empty, id);
    }

    /// <summary>
    /// Get Plant by Id
    /// </summary>
    /// <returns>Returns a single plant with provided Id</returns>
    [EndpointSummary("Updates Plant")]
    [EndpointDescription("Updates plant by id")]
    [EndpointName("UpdatePlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpPatch("{id:guid}", Name = "UpdatePlantById")]
    public async Task<ActionResult<PlantResponseModel>> UpdatePlantById([FromRoute] Guid id, [FromBody] UpdatePlantRequestModel updatePlantRequestModel)
    {
        var plant = await plantService.UpdatePlantAsync(updatePlantRequestModel.ToPlantModel(id));
        return Ok(plant);
    }

    /// <summary>
    /// Delete Plant
    /// </summary>
    /// <returns>204 if successful</returns>
    [EndpointSummary("Delete Plant")]
    [EndpointDescription("Deletes a Plant by id")]
    [EndpointName("DeletePlantById")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status204NoContent, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpDelete("{id:guid}", Name = "DeletePlantById")]
    public async Task<ActionResult<PlantResponseModel>> DeletePlantById([FromRoute] Guid id)
    {
        await plantService.DeletePlantAsync(id);
        return NoContent();
    }
}