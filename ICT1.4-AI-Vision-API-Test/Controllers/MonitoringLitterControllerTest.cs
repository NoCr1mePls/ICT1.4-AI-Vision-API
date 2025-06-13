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
    public async Task Get_ReturnsOkResult_WithJsonString()
    {
        // Arrange
        var userId = new Guid();
        var jsonResult = "testJson";
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync(jsonResult);
        int? cat = 0;

        // Act
        var result = await _controller.Get(new DateOnly?(), cat);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(jsonResult, okResult.Value);
    }

    [TestMethod]
    public async Task Get_ReturnsBadRequest_WhenJsonIsNull()
    {
        // Arrange
        var userId = "testUserId";
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync((string)null);

        // Act
        var result = await _controller.Get();

        // Assert
        var badRequestResult = result.Result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
    }
}
