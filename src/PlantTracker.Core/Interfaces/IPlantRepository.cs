using PlantTracker.Core.Models;

namespace PlantTracker.Core.Interfaces;

public interface IPlantRepository
{
    Task<IEnumerable<PlantModel>> GetAllPlantsAsync();
    Task<PlantModel?> GetPlantByIdAsync(Guid id);
    Task<PlantModel> UpdatePlant(PlantModel updatedPlant);
}