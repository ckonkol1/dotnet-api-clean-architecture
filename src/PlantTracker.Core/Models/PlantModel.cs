using PlantTracker.Core.Constants;

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
}