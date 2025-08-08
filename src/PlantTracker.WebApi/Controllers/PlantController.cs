using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using System.Net;

namespace PlantTracker.WebApi.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("/v{v:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PlantController(ILogger<PlantController> logger, IPlantService plantService) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = "GetAllPlants")]
        public async Task<ActionResult<IEnumerable<PlantResponseModel>>> GetAllPlants()
        {
            var result = new List<PlantResponseModel>();
            var errorResponse = new ErrorResponse();
            try
            {
                result = (await plantService.GetAllPlantsAsync()).ToList();

                if (result?.Any() != true)
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
}