using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlantTracker.Core.Interfaces;
using PlantTracker.Core.Models;
using PlantTracker.WebApi.Controllers;

namespace PlantTracker.WebApi.UnitTests
{
    public class PlantControllerUnitTests
    {
        public class PlantsControllerTests
        {
            private readonly Mock<IPlantService> _mockPlantService;
            private readonly Mock<ILogger<PlantController>> _mockLogger;
            private readonly PlantController _controller;

            public PlantsControllerTests()
            {
                _mockPlantService = new Mock<IPlantService>();
                _mockLogger = new Mock<ILogger<PlantController>>();
                _controller = new PlantController(_mockLogger.Object, _mockPlantService.Object);
            }

            [Fact]
            public async Task GetAllPlants_WithPlants_ReturnsOkWithPlants()
            {
                var expectedPlants = new List<PlantResponseModel>
                {
                    new()
                    {
                        Id = new Guid("ec60872a-cad7-443b-ac94-4bb24a275633"),
                        CommonName = "Zebra Plant",
                        ScientificName = "Calathea zebrina",
                        Age = 3,
                        Duration = "Perennial",
                        Url = "https://plants.usda.gov/plant-profile/CAZE",
                    },
                    new()
                    {
                        Id = new Guid("e19588bb-ecf0-480b-988d-f7c74de6f935"),
                        CommonName = "Arizona hedgehog cactus",
                        ScientificName = "Echinocereus coccineus Engelm",
                        Age = 1,
                        Duration = "Perennial",
                        Url = "https://plants.usda.gov/plant-profile/ECCOA",
                    }
                };

                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ReturnsAsync(expectedPlants);

                var result = await _controller.GetAllPlants();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var actualPlants = Assert.IsAssignableFrom<List<PlantResponseModel>>(okResult.Value);

                Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
                Assert.Equal(2, actualPlants.Count);
                Assert.Equal("Zebra Plant", actualPlants[0].CommonName);
                Assert.Equal("Arizona hedgehog cactus", actualPlants[1].CommonName);

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
            }

            [Fact]
            public async Task GetAllPlants_WithEmptyList_ReturnsNotFound()
            {
                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ReturnsAsync((List<PlantResponseModel>) []);

                var result = await _controller.GetAllPlants();

                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
                Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
            }

            [Fact]
            public async Task GetAllPlants_WithNullResult_ReturnsNotFound()
            {
                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ReturnsAsync((List<PlantResponseModel>) []);

                var result = await _controller.GetAllPlants();

                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
                Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
            }

            [Fact]
            public async Task GetAllPlants_ServiceThrowsException_ReturnsInternalServerError()
            {
                var exceptionMessage = "Database connection failed";
                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ThrowsAsync(new Exception(exceptionMessage));

                var result = await _controller.GetAllPlants();

                var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
                Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

                var actualResult = Assert.IsAssignableFrom<List<PlantResponseModel>>(statusCodeResult.Value);
                Assert.Empty(actualResult);

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);

                VerifyLoggerWasCalled(_mockLogger, LogLevel.Information, "Error Occurred");
                VerifyLoggerWasCalled(_mockLogger, LogLevel.Information, "Error Occurred");
            }

            [Fact]
            public async Task GetAllPlants_ServiceThrowsSpecificException_LogsAndReturnsError()
            {
                var specificException = new InvalidOperationException("Specific database error");
                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ThrowsAsync(specificException);

                var result = await _controller.GetAllPlants();

                var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
                Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

                VerifyLoggerWasCalled(_mockLogger, LogLevel.Information, "Error Occurred", Times.Once());

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Once);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(5)]
            [InlineData(10)]
            public async Task GetAllPlants_WithVariousPlantCounts_ReturnsCorrectCount(int plantCount)
            {
                var plants = Enumerable.Range(1, plantCount)
                    .Select(i => new PlantResponseModel
                    {
                        Id = new Guid("ec60872a-cad7-443b-ac94-4bb24a275633"),
                        CommonName = "Zebra Plant",
                        ScientificName = "Calathea zebrina",
                        Age = 3,
                        Duration = "Perennial",
                        Url = "https://plants.usda.gov/plant-profile/CAZE",
                    })
                    .ToList();

                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ReturnsAsync(plants);


                var result = await _controller.GetAllPlants();


                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var actualPlants = Assert.IsAssignableFrom<List<PlantResponseModel>>(okResult.Value);

                Assert.Equal(plantCount, actualPlants.Count);
                Assert.Equal($"Zebra Plant", actualPlants[0].CommonName);
            }

            [Fact]
            public async Task GetAllPlants_SinglePlant_ReturnsOkWithSingleItem()
            {
                var singlePlant = new List<PlantResponseModel>
                {
                    new()
                    {
                        Id = new Guid("ec60872a-cad7-443b-ac94-4bb24a275633"),
                        CommonName = "Zebra Plant",
                        ScientificName = "Calathea zebrina",
                        Age = 3,
                        Duration = "Perennial",
                        Url = "https://plants.usda.gov/plant-profile/CAZE",
                    }
                };

                _mockPlantService.Setup(x => x.GetAllPlantsAsync())
                    .ReturnsAsync(singlePlant);


                var result = await _controller.GetAllPlants();


                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var actualPlants = Assert.IsAssignableFrom<List<PlantResponseModel>>(okResult.Value);

                Assert.Single(actualPlants);
                Assert.Equal("Zebra Plant", actualPlants[0].CommonName);
            }

            [Fact]
            public async Task GetAllPlants_MultipleExceptions_HandlesCorrectly()
            {
                _mockPlantService.SetupSequence(x => x.GetAllPlantsAsync())
                    .ThrowsAsync(new Exception("First error"))
                    .ThrowsAsync(new Exception("Second error"));

                var result1 = await _controller.GetAllPlants();
                var result2 = await _controller.GetAllPlants();

                var statusResult1 = Assert.IsType<ObjectResult>(result1.Result);
                var statusResult2 = Assert.IsType<ObjectResult>(result2.Result);

                Assert.Equal(StatusCodes.Status500InternalServerError, statusResult1.StatusCode);
                Assert.Equal(StatusCodes.Status500InternalServerError, statusResult2.StatusCode);

                _mockPlantService.Verify(x => x.GetAllPlantsAsync(), Times.Exactly(2));

                VerifyLoggerWasCalled(_mockLogger, LogLevel.Information, "Error Occurred", Times.Exactly(2));
            }

            private static void VerifyLoggerWasCalled<T>(
                Mock<ILogger<T>> mockLogger,
                LogLevel expectedLogLevel,
                string expectedMessage,
                Times? times = null)
            {
                times ??= Times.Once();
                mockLogger.Verify(
                    x => x.Log(
                        expectedLogLevel,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains(expectedMessage)),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception?, string>>()), (Times)times);
            }
        }
    }
}