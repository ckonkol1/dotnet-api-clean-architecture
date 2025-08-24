using Amazon.DynamoDBv2.Model;
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
    /// Get Plant By Id
    /// </summary>
    /// <returns>A list of all the Plants</returns>
    [EndpointSummary("Get Plant by Id")]
    [EndpointDescription("This endpoint returns a plant with the provided Id")]
    [EndpointName("GetPlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [HttpGet("{id}", Name = "GetPlantById")]
    public async Task<ActionResult<PlantResponseModel>> GetPlantById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var plantId))
        {
            throw new ArgumentException($"Invalid GUID format: {id}");
        }

        if (plantId == Guid.Empty)
        {
            throw new ArgumentException("Plant Id cannot be an empty GUID");
        }

        var plant = await plantService.GetPlantByIdAsync(plantId);
        if (plant == null)
        {
            throw new ResourceNotFoundException($"Plant Id: {plantId} was not found");
        }

        return Ok(plant);
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
    /// Update Plant by Id
    /// </summary>
    /// <returns>Returns a single plant with provided Id</returns>
    [EndpointSummary("Updates Plant")]
    [EndpointDescription("Updates plant by id")]
    [EndpointName("UpdatePlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpPatch("{id}", Name = "UpdatePlantById")]
    public async Task<ActionResult<PlantResponseModel>> UpdatePlantById([FromRoute] string id, [FromBody] UpdatePlantRequestModel updatePlantRequestModel)
    {
        if (!Guid.TryParse(id, out var plantId))
        {
            throw new ArgumentException($"Invalid GUID format: {id}");
        }

        if (plantId == Guid.Empty)
        {
            throw new ArgumentException("Plant Id cannot be an empty GUID");
        }

        var plant = await plantService.UpdatePlantAsync(updatePlantRequestModel.ToPlantModel(plantId));
        return Ok(plant);
    }

    /// <summary>
    /// Delete Plant
    /// </summary>
    /// <returns>l</returns>
    [EndpointSummary("Delete Plant")]
    [EndpointDescription("Deletes a Plant by id")]
    [EndpointName("DeletePlantById")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status204NoContent, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [HttpDelete("{id}", Name = "DeletePlantById")]
    public async Task<ActionResult<PlantResponseModel>> DeletePlantById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out var plantId))
        {
            throw new ArgumentException($"Invalid GUID format: {id}");
        }

        if (plantId == Guid.Empty)
        {
            throw new ArgumentException("Plant Id cannot be an empty GUID");
        }

        await plantService.DeletePlantAsync(plantId);
        return NoContent();
    }
}