using PlantTracker.Core.Constants;
using PlantTracker.Core.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantTracker.Core.Models;

public class UpdatePlantRequestModel
{
    [property: Description("Common plant name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Common Name can only contain letters")]
    public string CommonName { get; set; }

    [property: Description("Scientific Plant Name")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Scientific Name can only contain letters")]
    public string ScientificName { get; set; }

    [property: Description("Duration of plant. Perennial or Annual")]
    [ValidEnum]
    public Duration Duration { get; set; }

    [property: Description("Age of plant in years")]
    public int Age { get; set; }

    [property: Description("Url to usda.gov plant documenation")]
    [UsdaPlantProfileUrlAttribute]
    public string Url { get; set; }

    public PlantModel ToPlantModel(Guid id)
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
}