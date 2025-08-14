using PlantTracker.Core.Models;

namespace PlantTracker.Core.Interfaces;

public interface IPlantService
{
    Task<IEnumerable<PlantResponseModel>> GetAllPlantsAsync();
    Task<PlantResponseModel?> GetPlantByIdAsync(Guid id);
    Task<PlantResponseModel?> UpdatePlant(PlantModel updatedPlant);
}