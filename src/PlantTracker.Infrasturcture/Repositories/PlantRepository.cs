using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.Infrastructure.Models;

namespace PlantTracker.Infrastructure.Repositories;

public class PlantRepository(IDynamoDBContext dynamoDbContext) : IPlantRepository
{
    public async Task<IEnumerable<PlantModel>> GetAllPlantsAsync()
    {
        try
        {
            var plants = await dynamoDbContext.ScanAsync<PlantEntity>(new List<ScanCondition>()).GetRemainingAsync();
            return plants.Select(p => p.ToPlantModel()).ToList();
        }
        catch (Exception e)
        {
            throw new InternalServerErrorException("Failed to Connect to the Database.");
        }
    }
}