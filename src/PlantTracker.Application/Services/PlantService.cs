using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.Application.Services;

public class PlantService(IPlantRepository plantRepository) : IPlantService
{
    public async Task<IEnumerable<PlantResponseModel>> GetAllPlantsAsync()
    {
        var result = await plantRepository.GetAllPlantsAsync();
        return result.Select(p => p.ToPlantResponseModel()).ToList();
    }
}