using PlantTracker.Core.Constants;
using PlantTracker.Core.Exceptions;

namespace PlantTracker.Core.Models;

public class PlantModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public int Age { get; set; }
    public Duration Duration { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateUtc { get; set; }
    public DateTimeOffset ModifiedDateUtc { get; set; }

    public PlantResponseModel ToPlantResponseModel()
    {
        try
        {
            return new PlantResponseModel()
            {
                Id = Id,
                CommonName = CommonName,
                ScientificName = ScientificName,
                Duration = Duration.ToString(),
                Age = Age,
                Url = Url
            };
        }
        catch (Exception ex)
        {
            throw new MappingException("Failed to map to PlantResponseModel: " + ex.Message);
        }
    }
}