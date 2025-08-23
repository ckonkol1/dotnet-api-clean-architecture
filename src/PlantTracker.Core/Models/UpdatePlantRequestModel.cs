using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PlantTracker.Core.Constants;
using PlantTracker.Core.Exceptions;
using PlantTracker.Core.Validations;

namespace PlantTracker.Core.Models;

public class UpdatePlantRequestModel
{
    [property: Description("Common plant name")]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Common Name can only contain letters")]
    public string CommonName { get; set; } = string.Empty;

    [property: Description("Scientific Plant Name")]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Scientific Name can only contain letters")]
    public string ScientificName { get; set; } = string.Empty;

    [property: Description("Duration of plant. Perennial or Annual")]
    [ValidEnum]
    public Duration Duration { get; set; }

    [property: Description("Age of plant in years")]
    public int Age { get; set; }

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