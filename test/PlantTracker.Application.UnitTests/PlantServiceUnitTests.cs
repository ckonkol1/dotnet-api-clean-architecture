using Moq;
using PlantTracker.Application.Services;
using PlantTracker.Core.Constants;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;

namespace PlantTracker.Application.UnitTests
{
    public class PlantServiceUnitTests
    {
        private readonly Mock<IPlantRepository> _mockPlantRepository;
        private readonly PlantService _plantService;
        private readonly DateTime _fixedDateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        public PlantServiceUnitTests()
        {
            _mockPlantRepository = new Mock<IPlantRepository>();
            _plantService = new PlantService(_mockPlantRepository.Object);
        }

        [Fact]
        public async Task GetAllPlantsAsync_WhenPlantsExist_ReturnsPlantResponseModels()
        {
            var plantModels = new List<PlantModel>
            {
                new PlantModel
                {
                    Id = Guid.NewGuid(),
                    CommonName = "Rose",
                    ScientificName = "Rosa rubiginosa",
                    Duration = Duration.Perennial,
                    Age = 2,
                    Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA",
                    CreatedDateUtc = _fixedDateTime,
                    ModifiedDateUtc = _fixedDateTime
                },
                new PlantModel
                {
                    Id = Guid.NewGuid(),
                    CommonName = "Tulip",
                    ScientificName = "Tulipa gesneriana",
                    Duration = Duration.Annual,
                    Age = 1,
                    Url = "https://plants.usda.gov/home/plantProfile?symbol=TUGE",
                    CreatedDateUtc = _fixedDateTime,
                    ModifiedDateUtc = _fixedDateTime
                }
            };

            _mockPlantRepository
                .Setup(x => x.GetAllPlantsAsync())
                .ReturnsAsync(plantModels);

            var result = await _plantService.GetAllPlantsAsync();

            var responseModels = result.ToList();
            Assert.Equal(2, responseModels.Count);
            Assert.Equal("Rose", responseModels[0].CommonName);
            Assert.Equal("Tulip", responseModels[1].CommonName);
            Assert.All(responseModels, model => Assert.IsType<PlantResponseModel>(model));
            _mockPlantRepository.Verify(x => x.GetAllPlantsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllPlantsAsync_WhenNoPlantsExist_ReturnsEmptyList()
        {
            _mockPlantRepository
                .Setup(x => x.GetAllPlantsAsync())
                .ReturnsAsync(new List<PlantModel>());

            var result = await _plantService.GetAllPlantsAsync();

            Assert.Empty(result);
            _mockPlantRepository.Verify(x => x.GetAllPlantsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllPlantsAsync_WhenRepositoryReturnsNull_ReturnsEmptyList()
        {
            _mockPlantRepository
                .Setup(x => x.GetAllPlantsAsync())
                .ReturnsAsync((IEnumerable<PlantModel>)null!);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _plantService.GetAllPlantsAsync());

            _mockPlantRepository.Verify(x => x.GetAllPlantsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPlantByIdAsync_WhenPlantExists_ReturnsPlantResponseModel()
        {
            var plantId = Guid.NewGuid();
            var plantModel = new PlantModel
            {
                Id = plantId,
                CommonName = "Rose",
                ScientificName = "Rosa rubiginosa",
                Duration = Duration.Perennial,
                Age = 2,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA",
                CreatedDateUtc = _fixedDateTime,
                ModifiedDateUtc = _fixedDateTime
            };

            _mockPlantRepository
                .Setup(x => x.GetPlantByIdAsync(plantId))
                .ReturnsAsync(plantModel);

            var result = await _plantService.GetPlantByIdAsync(plantId);

            Assert.NotNull(result);
            Assert.Equal(plantId, result.Id);
            Assert.Equal("Rose", result.CommonName);
            Assert.Equal("Rosa rubiginosa", result.ScientificName);
            Assert.IsType<PlantResponseModel>(result);
            _mockPlantRepository.Verify(x => x.GetPlantByIdAsync(plantId), Times.Once);
        }

        [Fact]
        public async Task GetPlantByIdAsync_WhenPlantDoesNotExist_ReturnsNull()
        {
            var plantId = Guid.NewGuid();

            _mockPlantRepository
                .Setup(x => x.GetPlantByIdAsync(plantId))
                .ReturnsAsync(null as PlantModel);

            var result = await _plantService.GetPlantByIdAsync(plantId);

            Assert.Null(result);
            _mockPlantRepository.Verify(x => x.GetPlantByIdAsync(plantId), Times.Once);
        }

        [Fact]
        public async Task GetPlantByIdAsync_WithEmptyGuid_CallsRepositoryAndReturnsNull()
        {
            var emptyId = Guid.Empty;

            _mockPlantRepository
                .Setup(x => x.GetPlantByIdAsync(emptyId))
                .ReturnsAsync(null as PlantModel);

            var result = await _plantService.GetPlantByIdAsync(emptyId);

            Assert.Null(result);
            _mockPlantRepository.Verify(x => x.GetPlantByIdAsync(emptyId), Times.Once);
        }

        [Fact]
        public async Task UpdatePlantAsync_WithValidPlant_ReturnsUpdatedPlantResponseModel()
        {
            var plantId = Guid.NewGuid();
            var inputPlantModel = new PlantModel
            {
                Id = plantId,
                CommonName = "Updated Rose",
                ScientificName = "Rosa updated",
                Duration = Duration.Annual,
                Age = 3,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2"
            };

            var updatedPlantModel = new PlantModel
            {
                Id = plantId,
                CommonName = "Updated Rose",
                ScientificName = "Rosa updated",
                Duration = Duration.Annual,
                Age = 3,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2",
                CreatedDateUtc = _fixedDateTime.AddDays(-1),
                ModifiedDateUtc = _fixedDateTime
            };

            _mockPlantRepository
                .Setup(x => x.UpdatePlantAsync(inputPlantModel))
                .ReturnsAsync(updatedPlantModel);

            var result = await _plantService.UpdatePlantAsync(inputPlantModel);

            Assert.NotNull(result);
            Assert.Equal(plantId, result.Id);
            Assert.Equal("Updated Rose", result.CommonName);
            Assert.Equal("Rosa updated", result.ScientificName);
            Assert.Equal(nameof(Duration.Annual), result.Duration);
            Assert.Equal(3, result.Age);
            Assert.IsType<PlantResponseModel>(result);
            _mockPlantRepository.Verify(x => x.UpdatePlantAsync(inputPlantModel), Times.Once);
        }

        [Fact]
        public async Task UpdatePlantAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            var plantModel = new PlantModel
            {
                Id = Guid.NewGuid(),
                CommonName = "Test Plant",
                ScientificName = "Test species",
                Duration = Duration.Perennial,
                Age = 1,
                Url = "https://plants.usda.gov/test"
            };

            var expectedException = new InvalidOperationException("Repository error");
            _mockPlantRepository
                .Setup(x => x.UpdatePlantAsync(plantModel))
                .ThrowsAsync(expectedException);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _plantService.UpdatePlantAsync(plantModel));

            Assert.Equal(expectedException.Message, exception.Message);
            _mockPlantRepository.Verify(x => x.UpdatePlantAsync(plantModel), Times.Once);
        }

        [Fact]
        public async Task CreatePlantAsync_WithValidPlant_ReturnsPlantId()
        {
            var plantModel = new PlantModel
            {
                CommonName = "Rose",
                ScientificName = "Rosa rubiginosa",
                Duration = Duration.Perennial,
                Age = 2,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA"
            };

            var expectedId = Guid.NewGuid().ToString();

            _mockPlantRepository
                .Setup(x => x.CreatePlantAsync(plantModel))
                .ReturnsAsync(expectedId);

            var result = await _plantService.CreatePlantAsync(plantModel);

            Assert.Equal(expectedId, result);
            _mockPlantRepository.Verify(x => x.CreatePlantAsync(plantModel), Times.Once);
        }

        [Fact]
        public async Task CreatePlantAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            var plantModel = new PlantModel
            {
                CommonName = "Rose",
                ScientificName = "Rosa rubiginosa",
                Duration = Duration.Perennial,
                Age = 2,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA"
            };

            var expectedException = new ArgumentException("Invalid plant data");
            _mockPlantRepository
                .Setup(x => x.CreatePlantAsync(plantModel))
                .ThrowsAsync(expectedException);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _plantService.CreatePlantAsync(plantModel));

            Assert.Equal(expectedException.Message, exception.Message);
            _mockPlantRepository.Verify(x => x.CreatePlantAsync(plantModel), Times.Once);
        }

        [Fact]
        public async Task DeletePlantAsync_WithValidId_CallsRepositoryDelete()
        {
            var plantId = Guid.NewGuid();

            _mockPlantRepository
                .Setup(x => x.DeletePlantAsync(plantId))
                .Returns(Task.CompletedTask);

            await _plantService.DeletePlantAsync(plantId);

            _mockPlantRepository.Verify(x => x.DeletePlantAsync(plantId), Times.Once);
        }

        [Fact]
        public async Task DeletePlantAsync_WithEmptyGuid_CallsRepositoryDelete()
        {
            var emptyId = Guid.Empty;

            _mockPlantRepository
                .Setup(x => x.DeletePlantAsync(emptyId))
                .Returns(Task.CompletedTask);

            await _plantService.DeletePlantAsync(emptyId);

            _mockPlantRepository.Verify(x => x.DeletePlantAsync(emptyId), Times.Once);
        }

        [Fact]
        public async Task DeletePlantAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            var plantId = Guid.NewGuid();
            var expectedException = new InvalidOperationException("Delete failed");

            _mockPlantRepository
                .Setup(x => x.DeletePlantAsync(plantId))
                .ThrowsAsync(expectedException);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _plantService.DeletePlantAsync(plantId));

            Assert.Equal(expectedException.Message, exception.Message);
            _mockPlantRepository.Verify(x => x.DeletePlantAsync(plantId), Times.Once);
        }

        [Fact]
        public async Task GetAllPlantsAsync_CallsRepositoryOnlyOnce()
        {
            var plantModels = new List<PlantModel>
            {
                new PlantModel
                {
                    Id = Guid.NewGuid(),
                    CommonName = "Rose",
                    ScientificName = "Rosa rubiginosa",
                    Duration = Duration.Perennial,
                    Age = 2,
                    Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA",
                    CreatedDateUtc = _fixedDateTime,
                    ModifiedDateUtc = _fixedDateTime
                }
            };

            _mockPlantRepository
                .Setup(x => x.GetAllPlantsAsync())
                .ReturnsAsync(plantModels);

            await _plantService.GetAllPlantsAsync();
            await _plantService.GetAllPlantsAsync();

            _mockPlantRepository.Verify(x => x.GetAllPlantsAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllPlantsAsync_EnsuresProperModelMapping()
        {
            var plantModel = new PlantModel
            {
                Id = Guid.NewGuid(),
                CommonName = "Rose",
                ScientificName = "Rosa rubiginosa",
                Duration = Duration.Perennial,
                Age = 2,
                Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA",
                CreatedDateUtc = _fixedDateTime,
                ModifiedDateUtc = _fixedDateTime.AddHours(1)
            };

            _mockPlantRepository
                .Setup(x => x.GetAllPlantsAsync())
                .ReturnsAsync(new List<PlantModel> { plantModel });

            var result = await _plantService.GetAllPlantsAsync();
            var responseModel = result.First();

            Assert.Equal(plantModel.Id, responseModel.Id);
            Assert.Equal(plantModel.CommonName, responseModel.CommonName);
            Assert.Equal(plantModel.ScientificName, responseModel.ScientificName);
            Assert.Equal(plantModel.Duration.ToString(), responseModel.Duration.ToString());
            Assert.Equal(plantModel.Age, responseModel.Age);
            Assert.Equal(plantModel.Url, responseModel.Url);
        }
    }
}