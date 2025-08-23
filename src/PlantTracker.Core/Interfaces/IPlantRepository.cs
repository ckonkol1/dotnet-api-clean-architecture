using PlantTracker.Core.Models;

namespace PlantTracker.Core.Interfaces;

public interface IPlantRepository
{
    Task<IEnumerable<PlantModel>> GetAllPlantsAsync();
    Task<PlantModel?> GetPlantByIdAsync(Guid id);
    Task<PlantModel> UpdatePlantAsync(PlantModel updatedPlant);
    Task<string> CreatePlantAsync(PlantModel plant);
    Task DeletePlantAsync(Guid id);
}