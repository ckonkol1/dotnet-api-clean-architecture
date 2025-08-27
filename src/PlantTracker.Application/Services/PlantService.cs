using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.Application.Services;

/// <summary>
/// PlantService
///
/// Makes calls to the IPlantRepository to perform Create, Read, Update, Delete operations
/// </summary>
/// <param name="plantRepository"></param>
public class PlantService(IPlantRepository plantRepository) : IPlantService
{
    /// <summary>
    /// GetAllPlantsAsync
    ///
    /// Get all plants from the plant repository
    /// </summary>
    /// <returns>List of PlantResponseModels objects</returns>
    public async Task<IEnumerable<PlantResponseModel>> GetAllPlantsAsync()
    {
        var result = await plantRepository.GetAllPlantsAsync();
        return result.Select(p => p.ToPlantResponseModel()).ToList();
    }

    /// <summary>
    /// GetPlantByIdAsync
    ///
    /// Get plant by Id from the plant repository
    /// </summary>
    /// <param name="id">Plant id of type guid</param>
    /// <returns>PlantResponseModel object</returns>
    public async Task<PlantResponseModel?> GetPlantByIdAsync(Guid id)
    {
        return (await plantRepository.GetPlantByIdAsync(id))?.ToPlantResponseModel();
    }

    /// <summary>
    /// UpdatePlantAsync
    ///
    /// Update existing plant in plant repository
    /// </summary>
    /// <param name="updatedPlant">PlantModel object</param>
    /// <returns>PlantResponseModel object</returns>
    public async Task<PlantResponseModel?> UpdatePlantAsync(PlantModel updatedPlant)
    {
        return (await plantRepository.UpdatePlantAsync(updatedPlant))?.ToPlantResponseModel();
    }

    /// <summary>
    /// CreatePlantAsync
    ///
    /// Creates a new plant in the plant repository
    /// </summary>
    /// <param name="plant"></param>
    /// <returns>Guid of plant id of type string</returns>
    public async Task<string> CreatePlantAsync(PlantModel plant)
    {
        return await plantRepository.CreatePlantAsync(plant);
    }

    /// <summary>
    /// DeletePlantAsync
    ///
    /// Hard Deletes a plant in the plant repository
    /// </summary>
    /// <param name="id">Guid id of plant</param>
    public async Task DeletePlantAsync(Guid id)
    {
        await plantRepository.DeletePlantAsync(id);
    }
}