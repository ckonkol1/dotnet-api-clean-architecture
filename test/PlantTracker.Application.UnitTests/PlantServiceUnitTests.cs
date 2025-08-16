using Moq;
using PlantTracker.Application.Services;
using PlantTracker.Core.Constants;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.Application.UnitTests
{
    public class PlantServiceUnitTests
    {
        [Fact]
        public async Task GetAllPlantsAsync_WhenPlantExists_ReturnsPlants()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);
            const string expectedPlantName = "Zebra Plant";

            mockRepository.Setup(x => x.GetAllPlantsAsync()).ReturnsAsync(new List<PlantModel>()
            {
                GetPlantModel()
            });

            var result = (await plantService.GetAllPlantsAsync()).ToList();


            Assert.Single(result);
            Assert.Equal(expectedPlantName, result[0].CommonName);
        }

        [Fact]
        public async Task GetAllPlantsAsync_WhenPlantDoesNotExist_ReturnsEmptyList()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);

            mockRepository.Setup(x => x.GetAllPlantsAsync()).ReturnsAsync(new List<PlantModel>() { });
            var result = (await plantService.GetAllPlantsAsync()).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPlantByIdAsync_WhenPlantDoesNotExist_PlantNotFound()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);

            mockRepository.Setup(x => x.GetPlantByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PlantModel?)null);

            var result = await plantService.GetPlantByIdAsync(It.IsAny<Guid>());
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPlantByIdAsync_WhenPlantExists_ReturnsPlant()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);
            var expectedPlant = GetPlantModel();

            mockRepository.Setup(x => x.GetPlantByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expectedPlant);

            var result = await plantService.GetPlantByIdAsync(It.IsAny<Guid>());
            var x = expectedPlant.ToPlantResponseModel();
            Assert.Equivalent(x, result);
        }

        [Fact]
        public async Task CreatePlant_WithValidPlant_PlantIsCreated()
        {
            var mockRepository = new Mock<IPlantRepository>();
            var plantService = new PlantService(mockRepository.Object);
            var expectedPlant = GetPlantModel();

            mockRepository.Setup(x => x.CreatePlantAsync(It.IsAny<PlantModel>())).ReturnsAsync(expectedPlant.Id.ToString());

            var result = await plantService.CreatePlantAsync(It.IsAny<PlantModel>());
            Assert.Equal(expectedPlant.Id.ToString(), result);
        }

        private static PlantModel GetPlantModel()
        {
            return new PlantModel()
            {
                Id = new Guid("ec60872a-cad7-443b-ac94-4bb24a275633"),
                CommonName = "Zebra Plant",
                ScientificName = "Calathea zebrina",
                Age = 3,
                Duration = Duration.Perennial,
                Url = "https://plants.usda.gov/plant-profile/CAZE",
                CreatedDateUtc = new DateTimeOffset(new DateTime(2025, 08, 16, 12, 00, 00)),
                ModifiedDateUtc = new DateTimeOffset(new DateTime(2025, 08, 16, 12, 00, 00)),
            };
        }
    }
}