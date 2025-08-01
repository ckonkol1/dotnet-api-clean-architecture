using Amazon.DynamoDBv2.DataModel;
using Moq;
using PlantTracker.Infrastructure.Models;
using PlantTracker.Infrastructure.Repositories;

namespace PlantTracker.Infrastructure.UnitTests
{
    public class PlantRepositoryUnitTests
    {
        [Fact]
        public async Task GetAllPlantsAsync_ReturnsPlantsSuccessfullyFromDatabase()
        {
            var mockDynamoDb = new Mock<IDynamoDBContext>();
            var plantRepository = new PlantRepository(mockDynamoDb.Object);
            const string expectedPlantName = "Zebra Plant";

            var expectedPlants = new List<PlantEntity>()
            {
                new PlantEntity()
                {
                    Id = "ec60872a-cad7-443b-ac94-4bb24a275633",
                    CommonName = "Zebra Plant",
                    ScientificName = "Calathea zebrina",
                    Age = 3,
                    Duration = "Perennial",
                    Url = "https://plants.usda.gov/plant-profile/CAZE",
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDateUtc = DateTime.UtcNow
                }
            };

            var mockAsyncSearch = new Mock<IAsyncSearch<PlantEntity>>();
            mockAsyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlants);

            mockDynamoDb.Setup(x => x.ScanAsync<PlantEntity>(It.IsAny<IEnumerable<ScanCondition>>()))
                .Returns(mockAsyncSearch.Object);

            var result = (await plantRepository.GetAllPlantsAsync()).ToList();


            Assert.Single(result);
            Assert.Equal(expectedPlantName, result[0].CommonName);
        }
    }
}