using PlantTrackerCleanArchitectureApi.Core.Models;

namespace PlantTrackerCleanArchitectureApi.Core.Interfaces;

public interface IPlantService
{
    Task<IEnumerable<PlantResponseModel>> GetAllPlantsAsync();
    Task<PlantResponseModel?> GetPlantByIdAsync(Guid id);
    Task<PlantResponseModel> UpdatePlantAsync(PlantModel updatedPlant);
    Task<string> CreatePlantAsync(PlantModel plant);
    Task DeletePlantAsync(Guid id);
}