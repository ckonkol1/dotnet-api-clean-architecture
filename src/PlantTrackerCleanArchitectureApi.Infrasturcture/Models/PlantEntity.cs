using Amazon.DynamoDBv2.DataModel;
using PlantTrackerCleanArchitectureApi.Core.Constants;
using PlantTrackerCleanArchitectureApi.Core.Exceptions;
using PlantTrackerCleanArchitectureApi.Core.Models;
using PlantTrackerCleanArchitectureApi.Infrastructure.Converters;

namespace PlantTrackerCleanArchitectureApi.Infrastructure.Models;

[DynamoDBTable("Plants")]
public class PlantEntity
{
    [DynamoDBHashKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty]
    public string CommonName { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string ScientificName { get; set; } = string.Empty;

    [DynamoDBProperty]
    public int Age { get; set; }

    [DynamoDBProperty]
    public string Duration { get; set; } = "Unknown";

    [DynamoDBProperty]
    public string Url { get; set; } = string.Empty;

    [DynamoDBProperty(Converter = typeof(DateTimeOffsetPropertyConverter))]
    public DateTimeOffset CreatedDateUtc { get; set; }

    [DynamoDBProperty(Converter = typeof(DateTimeOffsetPropertyConverter))]
    public DateTimeOffset ModifiedDateUtc { get; set; }

    public PlantModel ToPlantModel()
    {
        try
        {
            return new PlantModel()
            {
                Id = new Guid(Id),
                CommonName = CommonName,
                ScientificName = ScientificName,
                Duration = Enum.TryParse<Duration>(Duration,
                    ignoreCase: true,
                    out var result)
                    ? result
                    : Core.Constants.Duration.Unknown,
                Age = Age,
                Url = Url,
                CreatedDateUtc = CreatedDateUtc,
                ModifiedDateUtc = ModifiedDateUtc
            };
        }
        catch (Exception ex)
        {
            throw new MappingException("Failed to map to PlantModel: " + ex.Message);
        }
    }
}