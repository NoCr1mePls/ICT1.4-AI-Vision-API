using Moq;
using Microsoft.AspNetCore.Mvc;
using HomeTry.Controllers;
using HomeTry.Interfaces;
using HomeTry.Repositories;
using HomeTry.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ICT1._4_AI_Vision_API_Test;

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
        var userId = new Guid();
        Litter returnLitter = new Litter();
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync(returnLitter);
        int? category = 0;

        // Act
        var result = await _controller.Get(new DateOnly?(), category);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetOfDate_ReturnsBadRequest_WhenLitterObjectIsNull()
    {
        // Arrange
        var userId = new Guid();
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync((Litter)null);
        int? category = 0;

        // Act
        var result = await _controller.Get(new DateOnly?(), category);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsOkResult_WithLitterObject()
    {
        // Arrange
        var userId = new Guid();
        Litter returnLitter = new Litter();
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync(returnLitter);
        int? category = 0;

        // Act
        var result = await _controller.Get(new DateOnly?(), new DateOnly?(), category);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(returnLitter, okResult.Value);
    }

    [TestMethod]
    public async Task GetBetweenRange_ReturnsBadRequest_WhenLitterObjectIsNull()
    {
        // Arrange
        var userId = new Guid();
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync((Litter)null);
        int? category = 0;

        // Act
        var result = await _controller.Get(new DateOnly?(), new DateOnly?(), category);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
    }
}
