using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.Infrastructure.Models;

namespace PlantTracker.Infrastructure.Repositories;

/// <summary>
/// PlantRepository
///
/// Preforms Create, read, update, delete operations on a DynamoDB database
/// </summary>
/// <param name="dynamoDbContext"></param>
/// <param name="timeProvider"></param>
public class PlantRepository(IDynamoDBContext dynamoDbContext, TimeProvider timeProvider) : IPlantRepository
{
    /// <summary>
    /// GetAllPlantsAsync
    ///
    /// Returns all plants in the Plant table from the DynamoDB database
    /// </summary>
    /// <returns>List of PlantModel objects</returns>
    public async Task<IEnumerable<PlantModel>> GetAllPlantsAsync()
    {
        var plants = await dynamoDbContext.ScanAsync<PlantEntity>(new List<ScanCondition>()).GetRemainingAsync();
        return plants.Select(p => p.ToPlantModel()).ToList();
    }

    /// <summary>
    /// GetPlantByIdAsync
    ///
    /// Returns a plant by given plant id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>PlantModel object</returns>
    public async Task<PlantModel?> GetPlantByIdAsync(Guid id)
    {
        var plant = await dynamoDbContext.LoadAsync<PlantEntity>(id.ToString(), CancellationToken.None);
        return plant?.ToPlantModel();
    }

    /// <summary>
    /// UpdatePlantAsync
    /// 
    /// Updates existing plant in the Plant table from the DynamoDB database
    /// </summary>
    /// <param name="updatedPlant">PlantModel object</param>
    /// <returns>Updated PlantModel object</returns>
    /// <exception cref="ResourceNotFoundException">Throws when the Id of the plant was not found in the DynamoDB database</exception>
    public async Task<PlantModel> UpdatePlantAsync(PlantModel updatedPlant)
    {
        var originalPlantWithUpdates = await GetPlantByIdAsync(updatedPlant.Id);
        if (originalPlantWithUpdates == null)
        {
            throw new ResourceNotFoundException($"Plant With Id {updatedPlant.Id} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.CommonName))
        {
            originalPlantWithUpdates.CommonName = updatedPlant.CommonName;
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.ScientificName))
        {
            originalPlantWithUpdates.ScientificName = updatedPlant.ScientificName;
        }

        if (Enum.IsDefined(updatedPlant.Duration) && (updatedPlant.Duration != originalPlantWithUpdates.Duration))
        {
            originalPlantWithUpdates.Duration = updatedPlant.Duration;
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.Url))
        {
            originalPlantWithUpdates.Url = updatedPlant.Url;
        }

        if (updatedPlant.Age != int.MinValue)
        {
            originalPlantWithUpdates.Age = updatedPlant.Age;
        }

        var plantEntity = new PlantEntity()
        {
            Id = originalPlantWithUpdates.Id.ToString(),
            CommonName = originalPlantWithUpdates.CommonName,
            ScientificName = originalPlantWithUpdates.ScientificName,
            Duration = originalPlantWithUpdates.Duration.ToString(),
            Url = originalPlantWithUpdates.Url,
            Age = originalPlantWithUpdates.Age,
            CreatedDateUtc = originalPlantWithUpdates.CreatedDateUtc,
            ModifiedDateUtc = timeProvider.GetUtcNow()
        };

        await dynamoDbContext.SaveAsync(plantEntity, CancellationToken.None);
        return plantEntity.ToPlantModel();
    }

    /// <summary>
    /// CreatePlantAsync
    /// 
    /// Creates a new plant object in the Plant table in the DynamoDB Database
    /// </summary>
    /// <param name="plant">PlantModel Object</param>
    /// <return>Guid Id of type string</return>
    public async Task<string> CreatePlantAsync(PlantModel plant)
    {
        var plantEntity = new PlantEntity()
        {
            Id = Guid.NewGuid().ToString(),
            CommonName = plant.CommonName,
            ScientificName = plant.ScientificName,
            Duration = plant.Duration.ToString(),
            Url = plant.Url,
            Age = plant.Age,
            CreatedDateUtc = timeProvider.GetUtcNow(),
            ModifiedDateUtc = timeProvider.GetUtcNow()
        };

        await dynamoDbContext.SaveAsync(plantEntity, CancellationToken.None);
        return plantEntity.Id;
    }

    /// <summary>
    /// DeletePlantAsync
    ///
    /// Hard Deletes plant by Id in the Plant table in the DynamoDB Database
    /// </summary>
    /// <param name="id">Guid of plant id</param>
    public async Task DeletePlantAsync(Guid id)
    {
        await dynamoDbContext.DeleteAsync<PlantEntity>(id.ToString(), CancellationToken.None);
    }
}