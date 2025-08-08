using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTracker.Core.Models;

public class PlantResponseModel
{
    [property: Required]
    [property: Description("Generated Id for the plant")]
    public Guid Id { get; set; }

    [property: Required]
    [property: Description("Common plant name")]
    public string CommonName { get; set; }

    [property: Required]
    [property: Description("Scientific Plant Name")]
    public required string ScientificName { get; set; }

    [property: Required]
    [property: Description("Duration of plant. Perennial or Annual")]
    public string Duration { get; set; }

    [property: Required]
    [property: Description("Age of plant in years")]
    public required int Age { get; set; }

    [property: Required]
    [property: Description("Url to usda.gov plant documenation")]
    public required string Url { get; set; }
}