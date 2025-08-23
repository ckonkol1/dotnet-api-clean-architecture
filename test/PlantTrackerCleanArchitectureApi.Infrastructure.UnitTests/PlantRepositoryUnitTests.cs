using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Moq;
using PlantTrackerCleanArchitectureApi.Core.Constants;
using PlantTrackerCleanArchitectureApi.Core.Models;
using PlantTrackerCleanArchitectureApi.Infrastructure.Models;
using PlantTrackerCleanArchitectureApi.Infrastructure.Repositories;

namespace PlantTrackerCleanArchitectureApi.Infrastructure.UnitTests;

public class PlantRepositoryTests
{
    private readonly Mock<IDynamoDBContext> _mockDynamoDbContext;
    private readonly Mock<TimeProvider> _mockTimeProvider;
    private readonly PlantRepository _repository;
    private readonly DateTime _fixedDateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public PlantRepositoryTests()
    {
        _mockDynamoDbContext = new Mock<IDynamoDBContext>();
        _mockTimeProvider = new Mock<TimeProvider>();
        _mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(_fixedDateTime);
        _repository = new PlantRepository(_mockDynamoDbContext.Object, _mockTimeProvider.Object);
    }

    [Fact]
    public async Task GetAllPlantsAsync_WhenPlantsExist_ReturnsPlantModels()
    {
        var plantEntities = new List<PlantEntity>
        {
            GetPlantEntity(Guid.NewGuid()),
            GetPlantEntity(Guid.NewGuid())
        };

        var mockAsyncSearch = new Mock<AsyncSearch<PlantEntity>>();
        mockAsyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(plantEntities);

        _mockDynamoDbContext
            .Setup(x => x.ScanAsync<PlantEntity>(It.IsAny<List<ScanCondition>>()))
            .Returns(mockAsyncSearch.Object);

        var result = await _repository.GetAllPlantsAsync();

        var plants = result.ToList();
        Assert.Equal(2, plants.Count);
        _mockDynamoDbContext.Verify(x => x.ScanAsync<PlantEntity>(It.IsAny<List<ScanCondition>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllPlantsAsync_WhenNoPlantsExist_ReturnsEmptyList()
    {
        var mockAsyncSearch = new Mock<AsyncSearch<PlantEntity>>();
        mockAsyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<PlantEntity>());

        _mockDynamoDbContext
            .Setup(x => x.ScanAsync<PlantEntity>(It.IsAny<List<ScanCondition>>()))
            .Returns(mockAsyncSearch.Object);

        var result = await _repository.GetAllPlantsAsync();

        Assert.Empty(result);
        _mockDynamoDbContext.Verify(x => x.ScanAsync<PlantEntity>(It.IsAny<List<ScanCondition>>()), Times.Once);
    }

    [Fact]
    public async Task GetPlantByIdAsync_WhenPlantExists_ReturnsPlantModel()
    {
        var plantId = Guid.NewGuid();
        var plantEntity = GetPlantEntity(plantId);

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))
            .ReturnsAsync(plantEntity);

        var result = await _repository.GetPlantByIdAsync(plantId);

        Assert.NotNull(result);
        Assert.Equal(plantId, result.Id);
        Assert.Equal("Rose", result.CommonName);
        Assert.Equal("Rosa rubiginosa", result.ScientificName);
        _mockDynamoDbContext.Verify(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetPlantByIdAsync_WhenPlantDoesNotExist_ReturnsNull()
    {
        var plantId = Guid.NewGuid();

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))!
            .ReturnsAsync(null as PlantEntity);

        var result = await _repository.GetPlantByIdAsync(plantId);

        Assert.Null(result);
        _mockDynamoDbContext.Verify(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreatePlantAsync_WithValidPlant_ReturnsPlantId()
    {
        var plantModel = GetPlantModel(Guid.NewGuid());

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _repository.CreatePlantAsync(plantModel);

        Assert.NotNull(result);
        Assert.True(Guid.TryParse(result, out _));
        _mockDynamoDbContext.Verify(x => x.SaveAsync(It.Is<PlantEntity>(p =>
            p.CommonName == "Updated Rose" &&
            p.ScientificName == "Rosa updated" &&
            p.Duration == "Annual" &&
            p.Age == 3 &&
            p.CreatedDateUtc == _fixedDateTime &&
            p.ModifiedDateUtc == _fixedDateTime
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantAsync_WhenPlantExists_UpdatesAndReturnsPlant()
    {
        var plantId = Guid.NewGuid();

        var updatedPlantData = GetPlantModel(plantId);
        var existingEntity = GetPlantEntity(plantId);

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))
            .ReturnsAsync(existingEntity);

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _repository.UpdatePlantAsync(updatedPlantData);

        Assert.Equivalent(updatedPlantData, result);

        _mockDynamoDbContext.Verify(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None), Times.Once);
        _mockDynamoDbContext.Verify(x => x.SaveAsync(It.Is<PlantEntity>(p =>
            p.Id == plantId.ToString() &&
            p.CommonName == "Updated Rose" &&
            p.ScientificName == "Rosa updated" &&
            p.Duration == "Annual" &&
            p.Age == 3
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantAsync_WhenPlantDoesNotExist_ThrowsResourceNotFoundException()
    {
        var plantId = Guid.NewGuid();
        var updatedPlantData = GetPlantModel(plantId);

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))!
            .ReturnsAsync(null as PlantEntity);

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            _repository.UpdatePlantAsync(updatedPlantData));

        Assert.Equal($"Plant With Id {plantId} was not found.", exception.Message);
        _mockDynamoDbContext.Verify(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None), Times.Once);
        _mockDynamoDbContext.Verify(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePlantAsync_WithPartialUpdate_UpdatesOnlyProvidedFields()
    {
        var plantId = Guid.NewGuid();
        var existingEntity = GetPlantEntity(plantId);

        var partialUpdate = GetPlantModel(plantId);
        partialUpdate.ScientificName = string.Empty;
        partialUpdate.Url = string.Empty;

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))
            .ReturnsAsync(existingEntity);

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _repository.UpdatePlantAsync(partialUpdate);

        Assert.Equal("Updated Rose", result.CommonName);
        Assert.Equal("Rosa rubiginosa", result.ScientificName); // Should remain unchanged
        Assert.Equal("https://plants.usda.gov/home/plantProfile?symbol=ROSA", result.Url); // Should remain unchanged
        Assert.Equal(Duration.Annual, result.Duration);
        Assert.Equal(3, result.Age);
    }

    [Fact]
    public async Task UpdatePlantAsync_WithInvalidDuration_DoesNotUpdateDuration()
    {
        var plantId = Guid.NewGuid();
        var existingEntity = GetPlantEntity(plantId);

        var updateWithInvalidDuration = GetPlantModel(plantId);
        updateWithInvalidDuration.Duration = (Duration)999;


        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))
            .ReturnsAsync(existingEntity);

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _repository.UpdatePlantAsync(updateWithInvalidDuration);

        Assert.Equal(Duration.Perennial, result.Duration);
    }

    [Fact]
    public async Task DeletePlantAsync_WithValidId_CallsDynamoDbDelete()
    {
        var plantId = Guid.NewGuid();

        _mockDynamoDbContext
            .Setup(x => x.DeleteAsync(plantId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repository.DeletePlantAsync(plantId);

        _mockDynamoDbContext.Verify(x => x.DeleteAsync(plantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePlantAsync_WithEmptyGuid_CallsDynamoDbDelete()
    {
        var emptyId = Guid.Empty;

        _mockDynamoDbContext
            .Setup(x => x.DeleteAsync(emptyId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repository.DeletePlantAsync(emptyId);

        _mockDynamoDbContext.Verify(x => x.DeleteAsync(emptyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePlantAsync_SetsCreatedAndModifiedDates()
    {
        var plantModel = GetPlantModel(Guid.NewGuid());

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repository.CreatePlantAsync(plantModel);

        _mockDynamoDbContext.Verify(x => x.SaveAsync(It.Is<PlantEntity>(p =>
            p.CreatedDateUtc == _fixedDateTime &&
            p.ModifiedDateUtc == _fixedDateTime
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantAsync_UpdatesModifiedDate()
    {
        var plantId = Guid.NewGuid();
        var existingEntity = GetPlantEntity(plantId);
        var updatedPlantData = GetPlantModel(plantId);

        _mockDynamoDbContext
            .Setup(x => x.LoadAsync<PlantEntity>(plantId.ToString(), CancellationToken.None))
            .ReturnsAsync(existingEntity);

        _mockDynamoDbContext
            .Setup(x => x.SaveAsync(It.IsAny<PlantEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _repository.UpdatePlantAsync(updatedPlantData);

        Assert.Equal(_fixedDateTime, result.ModifiedDateUtc);
        Assert.Equal(_fixedDateTime, result.CreatedDateUtc);
    }

    private PlantModel GetPlantModel(Guid id)
    {
        return new PlantModel
        {
            Id = id,
            CommonName = "Updated Rose",
            ScientificName = "Rosa updated",
            Duration = Duration.Annual,
            Age = 3,
            Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA2",
            CreatedDateUtc = _fixedDateTime,
            ModifiedDateUtc = _fixedDateTime
        };
    }

    private PlantEntity GetPlantEntity(Guid id)
    {
        return new PlantEntity
        {
            Id = id.ToString(),
            CommonName = "Rose",
            ScientificName = "Rosa rubiginosa",
            Duration = "Perennial",
            Age = 2,
            Url = "https://plants.usda.gov/home/plantProfile?symbol=ROSA",
            CreatedDateUtc = _fixedDateTime,
            ModifiedDateUtc = _fixedDateTime
        };
    }
}