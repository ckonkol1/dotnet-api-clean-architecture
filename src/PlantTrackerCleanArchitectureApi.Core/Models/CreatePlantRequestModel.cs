using PlantTrackerCleanArchitectureApi.Core.Constants;
using PlantTrackerCleanArchitectureApi.Core.Exceptions;
using PlantTrackerCleanArchitectureApi.Core.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTrackerCleanArchitectureApi.Core.Models;

public class CreatePlantRequestModel
{
    [property: Description("Common plant name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Common Name can only contain letters")]
    [Required]
    public string CommonName { get; set; } = string.Empty;

    [property: Description("Scientific Plant Name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Scientific Name can only contain letters")]
    [Required]
    public string ScientificName { get; set; } = string.Empty;

    [property: Description("Duration of plant. Perennial or Annual")]
    [ValidEnum]
    [Required]
    public Duration Duration { get; set; }

    [property: Description("Age of plant in years")]
    [Required]
    public int Age { get; set; }

    [property: Description("Url to usda.gov plant documenation")]
    [MaxLength(200)]
    [UsdaPlantProfileUrl]
    [Required]
    public string Url { get; set; } = string.Empty;

    public PlantModel ToPlantModel()
    {
        try
        {
            return new PlantModel()
            {
                CommonName = CommonName,
                ScientificName = ScientificName,
                Duration = Duration,
                Age = Age,
                Url = Url
            };
        }
        catch (Exception ex)
        {
            throw new MappingException("Failed to map to PlantModel: " + ex.Message);
        }
    }
}