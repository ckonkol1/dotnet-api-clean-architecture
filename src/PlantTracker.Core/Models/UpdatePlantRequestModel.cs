using PlantTracker.Core.Constants;
using PlantTracker.Core.Exceptions;
using PlantTracker.Core.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTracker.Core.Models;

public class UpdatePlantRequestModel
{
    [property: Description("Common plant name")]
    [OptionalStringLength(2, 100)]
    [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Common Name can only contain letters and spaces")]
    public string CommonName { get; set; } = string.Empty;

    [property: Description("Scientific Plant Name")]
    [OptionalStringLength(2, 100)]
    [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Scientific Name can only contain letters and spaces")]
    public string ScientificName { get; set; } = string.Empty;

    [property: Description("Duration of plant. Perennial or Annual")]
    [ValidEnum]
    public Duration Duration { get; set; }

    [property: Description("Age of plant in years")]
    [Range(1, 500, ErrorMessage = "Age must be between 1 and 500")]
    public int? Age { get; set; }

    [property: Description("Url to usda.gov plant documenation")]
    [MaxLength(200)]
    [UsdaPlantProfileUrl]
    public string Url { get; set; } = string.Empty;

    public PlantModel ToPlantModel(Guid id)
    {
        try
        {
            return new PlantModel()
            {
                Id = id,
                CommonName = CommonName,
                ScientificName = ScientificName,
                Duration = Duration,
                Age = Age ?? int.MinValue,
                Url = Url
            };
        }
        catch (Exception ex)
        {
            throw new MappingException("Failed to map to PlantResponseModel: " + ex.Message);
        }
    }
}