using Moq;
using Microsoft.AspNetCore.Mvc;
using HomeTry.Controllers;
using HomeTry.Repositories;
using HomeTry.Models;

namespace ICT1._4_AI_Vision_API_Test;

[TestClass]
public class MonitoringLitterControllerTest
{
    private Mock<LitterRepository> _mockRepo;
    private Mock<AuthenticationService> _mockAuth;
    private MonitoringLitterController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<LitterRepository>();
        _mockAuth = new Mock<AuthenticationService>();
        _controller = new MonitoringLitterController(_mockRepo.Object, _mockAuth.Object);
    }

    [TestMethod]
    public async Task Get_ReturnsOkResult_WithJsonString()
    {
        // Arrange
        var userId = "null";
        var jsonResult = "testJson";
        _mockAuth.Setup(auth => auth.GetCurrentAuthenticatedUserID()).Returns(userId);
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
        _mockAuth.Setup(auth => auth.GetCurrentAuthenticatedUserID()).Returns(userId);
        _mockRepo.Setup(repo => repo.ReadAsync(userId)).ReturnsAsync((string)null);

        // Act
        var result = await _controller.Get();

        // Assert
        var badRequestResult = result.Result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
    }
}
