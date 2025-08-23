using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTrackerCleanArchitectureApi.Core.Models;

public class PlantResponseModel
{
    [property: Required]
    [property: Description("Generated Id for the plant")]
    public Guid Id { get; set; }

    [property: Required]
    [property: Description("Common plant name")]
    public string CommonName { get; set; } = string.Empty;

    [property: Required]
    [property: Description("Scientific Plant Name")]
    public string ScientificName { get; set; } = string.Empty;

    [property: Required]
    [property: Description("Duration of plant")]
    public string Duration { get; set; } = "Unknown";

    [property: Required]
    [property: Description("Age of plant in years")]
    public int Age { get; set; }

    [property: Required]
    [property: Description("Url to usda.gov plant documenation")]
    public string Url { get; set; } = string.Empty;
}