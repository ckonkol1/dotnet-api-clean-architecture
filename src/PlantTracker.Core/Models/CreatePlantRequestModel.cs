using PlantTracker.Core.Constants;
using PlantTracker.Core.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTracker.Core.Models;

public class CreatePlantRequestModel
{
    [property: Description("Common plant name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Common Name can only contain letters")]
    [Required]
    public string CommonName { get; set; }

    [property: Description("Scientific Plant Name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Scientific Name can only contain letters")]
    [Required]
    public string ScientificName { get; set; }

    [property: Description("Duration of plant. Perennial or Annual")]
    [ValidEnum]
    [Required]
    public Duration Duration { get; set; }

    [property: Description("Age of plant in years")]
    [Required]
    public int Age { get; set; }

    [property: Description("Url to usda.gov plant documenation")]
    [UsdaPlantProfileUrlAttribute]
    [Required]
    public string Url { get; set; }
}