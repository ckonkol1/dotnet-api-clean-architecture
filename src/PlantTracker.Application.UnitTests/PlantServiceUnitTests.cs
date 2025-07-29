using Moq;
using PlantTracker.Application.Services;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.Application.UnitTests
{
    public class PlantServiceUnitTests
    {
        [Fact]
        public async Task GetAllPlantsAsync_ReturnsPlantsSuccessfully()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);
            const string expectedPlantName = "Zebra Plant";

            mockRepository.Setup(x => x.GetAllPlantsAsync()).ReturnsAsync(new List<PlantModel>()
            {
                new PlantModel()
                {
                    Id = new Guid("ec60872a-cad7-443b-ac94-4bb24a275633"),
                    CommonName = "Zebra Plant",
                    ScientificName = "Calathea zebrina",
                    Age = 3,
                    Duration = "Perennial",
                    Url = "https://plants.usda.gov/plant-profile/CAZE",
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDateUtc = DateTime.UtcNow
                }
            });

            var result = (await plantService.GetAllPlantsAsync()).ToList();


            Assert.Single(result);
            Assert.Equal(expectedPlantName, result[0].CommonName);
        }
    }
}