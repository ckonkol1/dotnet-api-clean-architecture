using Amazon.DynamoDBv2.Model;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.WebApi.Middleware.Identity;

namespace PlantTracker.WebApi.Controllers;

/// <summary>
/// Plant Controller
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class PlantsController(IPlantService plantService) : ControllerBase
{
    /// <summary>
    /// GetAllPlants
    ///
    /// Returns all plants
    /// </summary>
    /// <returns>List of PlantResponseModel objects</returns>
    [EndpointSummary("Get All Plants")]
    [EndpointDescription("This endpoint returns all plants stored in the database")]
    [EndpointName("GetAllPlants")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [Authorize]
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
    /// GetPlantById
    ///
    /// Get Plant By Id
    /// </summary>
    /// <returns>PlantResponseModel object</returns>
    [EndpointSummary("Get Plant by Id")]
    [EndpointDescription("This endpoint returns a plant with the provided Id")]
    [EndpointName("GetPlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [Authorize]
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
    /// CreatePlantAsync
    ///
    /// Creates a new Plant
    /// </summary>
    /// <returns>string guid Id of the new Plant</returns>
    [EndpointSummary("Create Plant")]
    [EndpointDescription("Creates a Plant")]
    [EndpointName("CreatePlant")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPut(Name = "CreatePlant")]
    public async Task<ActionResult<string>> CreatePlant([FromBody] CreatePlantRequestModel createPlantRequestModel)
    {
        var id = await plantService.CreatePlantAsync(createPlantRequestModel.ToPlantModel());
        return Created(string.Empty, id);
    }

    /// <summary>
    /// UpdatePlantById
    ///
    /// Updates a Plant by Id
    /// </summary>
    /// <returns>PlantResponseModel object of updated plant</returns>
    [EndpointSummary("Updates Plant")]
    [EndpointDescription("Updates plant by id")]
    [EndpointName("UpdatePlantById")]
    [ProducesResponseType<PlantResponseModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [Authorize]
    [RequiresClaim(IdentityData.AdminUserClaimName, "true")]
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
    /// DeletePlantById
    /// 
    /// Hard Deletes a Plant by Id
    /// </summary>
    [EndpointSummary("Delete Plant")]
    [EndpointDescription("Deletes a Plant by id")]
    [EndpointName("DeletePlantById")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status204NoContent, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
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