using Amazon.DynamoDBv2.DataModel;
using PlantTracker.Core.Models;

namespace PlantTracker.Infrastructure.Models;

[DynamoDBTable("Plants")]
public class PlantEntity
{
    [DynamoDBHashKey]
    public required string Id { get; set; }

    [DynamoDBProperty]
    public required string CommonName { get; set; }

    [DynamoDBProperty]
    public required string ScientificName { get; set; }

    [DynamoDBProperty]
    public required int Age { get; set; }

    [DynamoDBProperty]
    public required string Duration { get; set; }

    [DynamoDBProperty]
    public required string Url { get; set; }

    [DynamoDBProperty]
    public required DateTime CreatedDateUtc { get; set; }

    [DynamoDBProperty]
    public required DateTime ModifiedDateUtc { get; set; }

    public PlantModel ToPlantModel()
    {
        return new PlantModel()
        {
            Id = new Guid(Id),
            CommonName = CommonName,
            ScientificName = ScientificName,
            Duration = Duration,
            Age = Age,
            Url = Url,
            CreatedDateUtc = CreatedDateUtc,
            ModifiedDateUtc = ModifiedDateUtc
        };
    }
}