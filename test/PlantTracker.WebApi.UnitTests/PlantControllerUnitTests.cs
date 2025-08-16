using Microsoft.AspNetCore.Mvc;
using Moq;
using PlantTracker.Core.Constants;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.WebApi.Controllers;

namespace PlantTracker.WebApi.UnitTests;

public class PlantsControllerTests
{
    private readonly Mock<IPlantService> _mockPlantService;
    private readonly PlantsController _controller;

    public PlantsControllerTests()
    {
        _mockPlantService = new Mock<IPlantService>();
        _controller = new PlantsController(_mockPlantService.Object);
    }

    [Fact]
    public async Task GetAllPlants_WhenPlantsExist_ReturnsOkWithPlants()
    {
        var expectedPlants = new List<PlantResponseModel>
        {
            new() { Id = Guid.NewGuid(), CommonName = "Rose", ScientificName = "Rosa rubiginosa" },
            new() { Id = Guid.NewGuid(), CommonName = "Tulip", ScientificName = "Tulipa gesneriana" }
        };

        _mockPlantService
            .Setup(x => x.GetAllPlantsAsync())
            .ReturnsAsync(expectedPlants);

        var result = await _controller.GetAllPlants();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var plants = Assert.IsAssignableFrom<IEnumerable<PlantResponseModel>>(okResult.Value);
        Assert.Equal(expectedPlants.Count, plants.Count());
        _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPlants_WhenNoPlantsExist_ReturnsNotFound()
    {
        _mockPlantService
            .Setup(x => x.GetAllPlantsAsync())
            .ReturnsAsync(new List<PlantResponseModel>());

        var result = await _controller.GetAllPlants();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No plants were found.", notFoundResult.Value);
        _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPlants_WhenServiceReturnsNull_ReturnsNotFound()
    {
        _mockPlantService
            .Setup(x => x.GetAllPlantsAsync())
            .ReturnsAsync(new List<PlantResponseModel>());

        var result = await _controller.GetAllPlants();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No plants were found.", notFoundResult.Value);
        _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPlantById_WhenPlantExists_ReturnsOk()
    {
        var plantId = Guid.NewGuid();
        var expectedPlant = new PlantResponseModel
        {
            Id = plantId,
            CommonName = "Rose",
            ScientificName = "Rosa rubiginosa"
        };

        _mockPlantService
            .Setup(x => x.GetPlantByIdAsync(plantId))
            .ReturnsAsync(expectedPlant);

        var result = await _controller.GetPlantById(plantId);

        var okResult = Assert.IsType<OkResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        _mockPlantService.Verify(x => x.GetPlantByIdAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetPlantById_WhenPlantDoesNotExist_ReturnsNotFound()
    {
        var plantId = Guid.NewGuid();
        _mockPlantService
            .Setup(x => x.GetPlantByIdAsync(plantId))
            .ReturnsAsync(null as PlantResponseModel);

        var result = await _controller.GetPlantById(plantId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Resource Not Found. Id: {plantId}", notFoundResult.Value);
        _mockPlantService.Verify(x => x.GetPlantByIdAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetPlantById_WithEmptyGuid_ReturnsNotFound()
    {
        var emptyId = Guid.Empty;
        _mockPlantService
            .Setup(x => x.GetPlantByIdAsync(emptyId))
            .ReturnsAsync(null as PlantResponseModel);

        var result = await _controller.GetPlantById(emptyId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Resource Not Found. Id: {emptyId}", notFoundResult.Value);
        _mockPlantService.Verify(x => x.GetPlantByIdAsync(emptyId), Times.Once);
    }

    [Fact]
    public async Task CreatePlant_WithValidModel_ReturnsCreated()
    {
        var createRequest = new CreatePlantRequestModel
        {
            CommonName = "Rose",
            ScientificName = "Rosa rubiginosa",
            Duration = Duration.Perennial,
            Age = 2,
            Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA"
        };
        var expectedId = Guid.NewGuid().ToString();

        _mockPlantService
            .Setup(x => x.CreatePlantAsync(It.IsAny<PlantModel>()))
            .ReturnsAsync(expectedId);

        var result = await _controller.CreatePlant(createRequest);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(expectedId, createdResult.Value);
        _mockPlantService.Verify(x => x.CreatePlantAsync(It.IsAny<PlantModel>()), Times.Once);
    }

    [Fact]
    public async Task CreatePlant_WithNullModel_ShouldHandleGracefully()
    {
        await Assert.ThrowsAsync<NullReferenceException>(() => _controller.CreatePlant((null as CreatePlantRequestModel)!));
    }

    [Fact]
    public async Task UpdatePlantById_WithValidModel_ReturnsOkWithUpdatedPlant()
    {
        var plantId = Guid.NewGuid();
        var updateRequest = new UpdatePlantRequestModel
        {
            CommonName = "Updated Rose",
            ScientificName = "Rosa rubiginosa",
            Duration = Duration.Perennial,
            Age = 3,
            Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2"
        };
        var updatedPlant = new PlantResponseModel
        {
            Id = plantId,
            CommonName = "Updated Rose",
            ScientificName = "Rosa rubiginosa"
        };

        _mockPlantService
            .Setup(x => x.UpdatePlantAsync(It.IsAny<PlantModel>()))
            .ReturnsAsync(updatedPlant);

        var result = await _controller.UpdatePlantById(plantId, updateRequest);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPlant = Assert.IsType<PlantResponseModel>(okResult.Value);
        Assert.Equal(updatedPlant.Id, returnedPlant.Id);
        Assert.Equal(updatedPlant.CommonName, returnedPlant.CommonName);
        _mockPlantService.Verify(x => x.UpdatePlantAsync(It.IsAny<PlantModel>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantById_WithEmptyGuid_CallsService()
    {
        var emptyId = Guid.Empty;
        var updateRequest = new UpdatePlantRequestModel
        {
            CommonName = "Updated Rose",
            ScientificName = "Rosa rubiginosa",
            Duration = Duration.Perennial,
            Age = 3,
            Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2"
        };
        var updatedPlant = new PlantResponseModel
        {
            Id = emptyId,
            CommonName = "Updated Rose",
            ScientificName = "Rosa rubiginosa"
        };

        _mockPlantService
            .Setup(x => x.UpdatePlantAsync(It.IsAny<PlantModel>()))
            .ReturnsAsync(updatedPlant);

        var result = await _controller.UpdatePlantById(emptyId, updateRequest);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _mockPlantService.Verify(x => x.UpdatePlantAsync(It.IsAny<PlantModel>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantById_WithNullModel_ShouldHandleGracefully()
    {
        var plantId = Guid.NewGuid();

        await Assert.ThrowsAsync<NullReferenceException>(() => _controller.UpdatePlantById(plantId, (null as UpdatePlantRequestModel)!));
    }

    [Fact]
    public async Task DeletePlantById_WithValidId_ReturnsNoContent()
    {
        var plantId = Guid.NewGuid();
        _mockPlantService
            .Setup(x => x.DeletePlantAsync(plantId))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeletePlantById(plantId);

        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(204, noContentResult.StatusCode);
        _mockPlantService.Verify(x => x.DeletePlantAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task DeletePlantById_WithEmptyGuid_CallsService()
    {
        var emptyId = Guid.Empty;
        _mockPlantService
            .Setup(x => x.DeletePlantAsync(emptyId))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeletePlantById(emptyId);

        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(204, noContentResult.StatusCode);
        _mockPlantService.Verify(x => x.DeletePlantAsync(emptyId), Times.Once);
    }

    [Fact]
    public async Task DeletePlantById_WhenServiceThrowsException_PropagatesException()
    {
        var plantId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Plant not found");

        _mockPlantService
            .Setup(x => x.DeletePlantAsync(plantId))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.DeletePlantById(plantId));
        Assert.Equal(expectedException.Message, exception.Message);
        _mockPlantService.Verify(x => x.DeletePlantAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetAllPlants_WhenServiceThrowsException_PropagatesException()
    {
        var expectedException = new InvalidOperationException("Database connection failed");
        _mockPlantService
            .Setup(x => x.GetAllPlantsAsync())
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAllPlants());
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task GetPlantById_WhenServiceThrowsException_PropagatesException()
    {
        var plantId = Guid.NewGuid();
        var expectedException = new ArgumentException("Invalid plant ID");
        _mockPlantService
            .Setup(x => x.GetPlantByIdAsync(plantId))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetPlantById(plantId));
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task CreatePlant_WhenServiceThrowsException_PropagatesException()
    {
        var createRequest = new CreatePlantRequestModel { CommonName = "Rose", ScientificName = "Rosa rubiginosa", Duration = Duration.Perennial, Age = 2, Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA" };
        var expectedException = new InvalidOperationException("Failed to create plant");
        _mockPlantService
            .Setup(x => x.CreatePlantAsync(It.IsAny<PlantModel>()))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CreatePlant(createRequest));
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task UpdatePlantById_WhenServiceThrowsException_PropagatesException()
    {
        var plantId = Guid.NewGuid();
        var updateRequest = new UpdatePlantRequestModel { CommonName = "Updated Rose", ScientificName = "Rosa rubiginosa", Duration = Duration.Perennial, Age = 3, Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2" };
        var expectedException = new InvalidOperationException("Failed to update plant");
        _mockPlantService
            .Setup(x => x.UpdatePlantAsync(It.IsAny<PlantModel>()))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.UpdatePlantById(plantId, updateRequest));
        Assert.Equal(expectedException.Message, exception.Message);
    }
}