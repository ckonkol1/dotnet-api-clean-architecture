using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using PlantTracker.Core.Constants;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.Infrastructure.Models;

namespace PlantTracker.Infrastructure.Repositories;

public class PlantRepository(IDynamoDBContext dynamoDbContext, TimeProvider timeProvider) : IPlantRepository
{
    public async Task<IEnumerable<PlantModel>> GetAllPlantsAsync()
    {
        try
        {
            var plants = await dynamoDbContext.ScanAsync<PlantEntity>(new List<ScanCondition>()).GetRemainingAsync();
            return plants.Select(p => p.ToPlantModel()).ToList();
        }
        catch (Exception)
        {
            throw new InternalServerErrorException("Failed to Connect to the Database.");
        }
    }

    public async Task<PlantModel?> GetPlantByIdAsync(Guid id)
    {
        var plant = await dynamoDbContext.LoadAsync<PlantEntity>(id.ToString(), CancellationToken.None);
        return plant?.ToPlantModel();
    }

    public async Task<PlantModel> UpdatePlant(PlantModel updatedPlant)
    {
        var originalPlant = await GetPlantByIdAsync(updatedPlant.Id);
        if (originalPlant == null)
        {
            throw new ResourceNotFoundException($"Plant With Id {updatedPlant.Id} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.CommonName))
        {
            originalPlant.CommonName = updatedPlant.CommonName;
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.ScientificName))
        {
            originalPlant.ScientificName = updatedPlant.ScientificName;
        }

        if (Enum.IsDefined(typeof(Duration), updatedPlant.Duration))
        {
            originalPlant.Duration = updatedPlant.Duration;
        }

        if (!string.IsNullOrWhiteSpace(updatedPlant.Url))
        {
            originalPlant.Url = updatedPlant.Url;
        }

        originalPlant.Age = updatedPlant.Age;
        originalPlant.ModifiedDateUtc = timeProvider.GetUtcNow();
        var plantEntity = new PlantEntity()
        {
            Id = updatedPlant.Id.ToString(),
            CommonName = updatedPlant.CommonName,
            ScientificName = updatedPlant.ScientificName,
            Duration = updatedPlant.Duration,
            Url = updatedPlant.Url,
            Age = updatedPlant.Age,
            CreatedDateUtc = updatedPlant.CreatedDateUtc
        };

        await dynamoDbContext.SaveAsync(originalPlant);
        return originalPlant;
    }
}