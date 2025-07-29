namespace PlantTracker.Core.Models;

public class PlantResponseModel
{
    public required Guid Id { get; set; }
    public required string CommonName { get; set; }
    public required string ScientificName { get; set; }
    public required string Duration { get; set; }
    public required int Age { get; set; }
    public required string Url { get; set; }
}