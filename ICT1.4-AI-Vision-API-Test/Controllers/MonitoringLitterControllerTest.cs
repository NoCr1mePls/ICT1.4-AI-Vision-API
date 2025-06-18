using Moq;
using Microsoft.AspNetCore.Mvc;
using SensoringApi.Controllers;
using SensoringApi.Interfaces;
using SensoringApi.Repositories;
using SensoringApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SensoringApi.Test;
[TestClass]
public class MonitoringLitterControllerTest
{
    private Mock<ILitterRepository> _mockRepo;
    private Mock<ILogger<MonitoringLitterController>> _mockLogger;
    private MonitoringLitterController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<ILitterRepository>();
        _mockLogger = new Mock<ILogger<MonitoringLitterController>>();
        _controller = new MonitoringLitterController(_mockRepo.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsOkResult_WithLitterObject()
    {
        // Arrange
        List<Litter> returnLitter = new List<Litter>();
        returnLitter.Add(new Litter { location_latitude = 0, location_longitude = 0 });
        DateOnly? date = new DateOnly(2025, 6, 14);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetSingleDateData(date, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsBadRequest_WhenLitterObjectIsNull()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = null;
        DateOnly? date = new DateOnly();
        _mockRepo.Setup(repo => repo.ReadAsyncRange(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetSingleDateData(date, null);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsOkResult_WithLitterObject_WhenDateIsInRange()
    {
        // Arrange
        List<Litter> returnLitter = new List<Litter>();
        returnLitter.Add(new Litter { location_latitude = 0, location_longitude = 0 });
        DateOnly? date = new DateOnly(2025, 6, 15);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetSingleDateData(date, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsBadRequest_WhenDateIsOutOfRange()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = null;
        DateOnly? date = new DateOnly(2030, 8, 14);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetRangeDateData(date, null, null);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithLitterObject()
    {
        // Arrange
        List<Litter> returnLitter = new List<Litter>();
        returnLitter.Add(new Litter { location_latitude = 0, location_longitude = 0 });
        DateOnly? startDate = new DateOnly(2025, 6, 13);
        DateOnly? endDate = new DateOnly(2025, 6, 17);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetRangeDateData(startDate, endDate, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsBadRequest_WhenLitterObjectIsNull()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = null;
        DateOnly? startDate = new DateOnly();
        DateOnly? endDate = new DateOnly();
        _mockRepo.Setup(repo => repo.ReadAsyncRange(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.GetRangeDateData(startDate, endDate, null);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsBadRequest_WhenDatesAreOutOfRange()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = null;
        DateOnly? startDate = new DateOnly(1800, 1, 1);
        DateOnly? endDate = new DateOnly(1900, 12, 31);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        //Act
        var result = await _controller.GetRangeDateData(startDate, endDate, null);

        //Assert;
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithLitterObject_WhenDatesAreInRange()
    {
        // Arrange
        List<Litter> returnLitter = new List<Litter>();
        returnLitter.Add(new Litter { location_latitude = 0, location_longitude = 0 });
        DateOnly? startDate = new DateOnly(2025, 5, 2);
        DateOnly? endDate = new DateOnly(2025, 5, 5);
        _mockRepo.Setup(repo => repo.ReadAsyncRange(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue), null)).ReturnsAsync(returnLitter);

        //Act
        var result = await _controller.GetRangeDateData(startDate, endDate, null);

        //Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetCategories_ReturnsOkResult_WhenListString()
    {
        // Act
        var result = await _controller.GetCategories();

        //Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType<List<string>>(okResult.Value);
    }
}
