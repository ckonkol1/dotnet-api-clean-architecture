namespace PlantTracker.Core.Models;

public class PlantModel
{
    public required Guid Id { get; set; }
    public required string CommonName { get; set; }
    public required string ScientificName { get; set; }
    public required int Age { get; set; }
    public required string Duration { get; set; }
    public required string Url { get; set; }
    public required DateTime CreatedDateUtc { get; set; }
    public required DateTime ModifiedDateUtc { get; set; }

    public PlantResponseModel ToPlantResponseModel()
    {
        return new PlantResponseModel()
        {
            Id = Id,
            CommonName = CommonName,
            ScientificName = ScientificName,
            Duration = Duration,
            Age = Age,
            Url = Url
        };
    }
}