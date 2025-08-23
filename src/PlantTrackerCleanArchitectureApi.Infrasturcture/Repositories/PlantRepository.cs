using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using PlantTrackerCleanArchitectureApi.Infrastructure.Models;

namespace PlantTrackerCleanArchitectureApi.Infrastructure.Repositories;

public class PlantRepository(IDynamoDBContext dynamoDbContext, TimeProvider timeProvider) : IPlantRepository
{
    public async Task<IEnumerable<PlantModel>> GetAllPlantsAsync()
    {
        var plants = await dynamoDbContext.ScanAsync<PlantEntity>(new List<ScanCondition>()).GetRemainingAsync();
        return plants.Select(p => p.ToPlantModel()).ToList();
    }

    public async Task<PlantModel?> GetPlantByIdAsync(Guid id)
    {
        var plant = await dynamoDbContext.LoadAsync<PlantEntity>(id.ToString(), CancellationToken.None);
        return plant?.ToPlantModel();
    }

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

        if (Enum.IsDefined(typeof(Duration), updatedPlant.Duration) && (updatedPlant.Duration != originalPlantWithUpdates.Duration))
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

        originalPlantWithUpdates.Age = updatedPlant.Age;
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

    public async Task DeletePlantAsync(Guid id)
    {
        await dynamoDbContext.DeleteAsync(id);
    }
}