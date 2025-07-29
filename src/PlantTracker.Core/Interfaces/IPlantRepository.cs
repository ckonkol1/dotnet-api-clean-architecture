using PlantTracker.Core.Models;

namespace PlantTracker.Core.Interfaces;

public interface IPlantRepository
{
    Task<IEnumerable<PlantModel>> GetAllPlantsAsync();
}