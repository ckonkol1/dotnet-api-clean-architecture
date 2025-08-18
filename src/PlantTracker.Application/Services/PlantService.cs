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

    public async Task<PlantResponseModel?> GetPlantByIdAsync(Guid id)
    {
        return (await plantRepository.GetPlantByIdAsync(id))?.ToPlantResponseModel();
    }

    public async Task<PlantResponseModel?> UpdatePlantAsync(PlantModel updatedPlant)
    {
        return (await plantRepository.UpdatePlantAsync(updatedPlant))?.ToPlantResponseModel();
    }

    public async Task<string> CreatePlantAsync(PlantModel plant)
    {
        return await plantRepository.CreatePlantAsync(plant);
    }

    public async Task DeletePlantAsync(Guid id)
    {
        await plantRepository.DeletePlantAsync(id);
    }
}