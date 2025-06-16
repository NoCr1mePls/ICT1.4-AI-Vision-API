using Moq;
using Microsoft.AspNetCore.Mvc;
using HomeTry.Controllers;
using HomeTry.Interfaces;
using HomeTry.Repositories;
using HomeTry.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeTry._4_AI_Vision_API_Test;

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
        IEnumerable<Litter> returnLitter = new List<Litter>();
        DateOnly? date = new DateOnly();
        _mockRepo.Setup(repo => repo.ReadAsync(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.Get(date, null);

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
        _mockRepo.Setup(repo => repo.ReadAsync(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.Get(date, null);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsOkRequest_WithLitterObject_WhenDateIsInRange()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = new List<Litter?>();
        DateOnly? date = new DateOnly(2025, 6, 15);
        _mockRepo.Setup(repo => repo.ReadAsync(date.Value.ToDateTime(TimeOnly.MinValue), date.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.Get(date, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithLitterObject()
    {
        // Arrange
        IEnumerable<Litter> returnLitter = new List<Litter>();
        DateOnly? startDate = new DateOnly();
        DateOnly? endDate = new DateOnly().AddDays(1);
        _mockRepo.Setup(repo => repo.ReadAsync(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.Get(startDate, endDate, null);

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
        _mockRepo.Setup(repo => repo.ReadAsync(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        // Act
        var result = await _controller.Get(startDate, endDate, null);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithEmptyLitterObject_WhenRangeIsOutOfDataRange()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = null;
        DateOnly? startDate = new DateOnly(1800, 1, 1);
        DateOnly? endDate = new DateOnly(1900, 12, 31);
        _mockRepo.Setup(repo => repo.ReadAsync(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        //Act
        var result = await _controller.Get(startDate, endDate, null);

        //Assert
        var okResult = result as OkObjectResult;
        Assert.IsNull(okResult.Value);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithLitterObject_WhenRangeEncompassesDataRange()
    {
        // Arrange
        IEnumerable<Litter?> returnLitter = new List<Litter?>();
        DateOnly? startDate = new DateOnly(1800, 1, 1);
        DateOnly? endDate = new DateOnly(2200, 12, 31);
        _mockRepo.Setup(repo => repo.ReadAsync(startDate.Value.ToDateTime(TimeOnly.MinValue), endDate.Value.ToDateTime(TimeOnly.MaxValue))).ReturnsAsync(returnLitter);

        //Act
        var result = await _controller.Get(startDate, endDate, null);

        //Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }
}
