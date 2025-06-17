using HomeTry.Controllers;
using HomeTry.Interfaces;
using Moq;
using Microsoft.Extensions.Logging;
using HomeTry._4_AI_Vision_API.Controllers;
using HomeTry.Models;
using Newtonsoft.Json;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System.Net;
using System.Net.Http;
using System.Text;

namespace HomeTry._4_AI_Vision_API_Test;

[TestClass]
public class SensoringLitterControllerTest
{
    private Mock<ILitterRepository> _mockRepo;
    private Mock<ILogger<SensoringLitterController>> _mockLogger;
    private SensoringLitterController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<ILitterRepository>();
        _mockLogger = new Mock<ILogger<SensoringLitterController>>();

        var mockFactory = new Mock<IHttpClientFactory>();
        //Fake JSON Response for Weather API
        var responseJson = JsonConvert.SerializeObject(new
        {
            current = new
            {
                temp_c = 19,
                humidity = 45,
                condition = new { text = "Sunny" }
            }
        });
        //Fake HTTPClient returns JSON
        var httpClient = new HttpClient(new FakeHttpMessageHandler(responseJson, HttpStatusCode.OK));
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("fake");
        mockConfig.Setup(c => c.GetSection("WeatherApiKey")).Returns(mockSection.Object);

        //Set up controller with al mocked dependenicies
        _controller = new SensoringLitterController(
            _mockRepo.Object,
            _mockLogger.Object,
            mockFactory.Object,
            mockConfig.Object
        );
    }

    /// <summary>
    /// Tests if Add() returns 201 Created when longitude and latitude are valid.
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Add_ReturnsCreatedAt_WhenInputIsValid()
    {
        //Arrange
        var litter = new Litter { location_latitude = 50, location_longitude = 5 };

        _mockRepo.Setup(r => r.InsertAsync(It.IsAny<Litter>(), It.IsAny<Weather>()))
                 .ReturnsAsync(litter);

        //Act
        var result = await _controller.Add(litter);

        //Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    /// <summary>
    /// Tests if Add() returns 400 Badrequest when bot latidtude and longitude are missing.
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLatitudeAndLongitudeAreMissing()
    {
        //Arrange
        var litter = new Litter
        {
            location_latitude = 0,
            location_longitude = 0
        };

        //Act
        var result = await _controller.Add(litter);

        //Asserts
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Tests if Add() returns 400 Badrequest when latitude is missing (0).
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLatitudeIsMissing()
    {
        //Arrange
        var litter = new Litter
        {
            location_latitude = 0,
            location_longitude = 5
        };

        //Act
        var result = await _controller.Add(litter);

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Tests if Add() returns 400 Badrequest when longitude is missing (0).
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLongitudeIsMissing()
    {
        //Arrange
        var litter = new Litter
        {
            location_latitude = 5,
            location_longitude = 0
        };

        //Act
        var result = await _controller.Add(litter);

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Test if Get() returns a 404 Not Found when the litter records is not found in the repo.
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Get_ReturnsNotFound_WhenLitterNotExists()
    {
        //Arrange
        var fakeId = Guid.NewGuid();
        _mockRepo.Setup(r => r.ReadAsync(fakeId)).ReturnsAsync((Litter)null);

        //Act
        var result = await _controller.Get(fakeId);

        //Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    /// <summary>
    /// Tests if Get() returns 200 OK and the litter objects when it exists.
    /// </summary>
    /// <returns></returns>

    [TestMethod]
    public async Task Get_ReturnsOk_WhenLitterExists()
    {
        //Arrange
        var id = Guid.NewGuid();
        var litter = new Litter { litter_id = id };
        _mockRepo.Setup(r => r.ReadAsync(id)).ReturnsAsync(litter);

        //Act
        var result = await _controller.Get(id);

        //Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(litter, okResult.Value);
    }

    /// <summary>
    /// Tests if Add() sets the Weather object fields correctly from the Weather API response.
    /// </summary>

    [TestMethod]
    public async Task Add_SetsWeatherFields_FromWeatherApi()
    {
        //Arrange
        var litter = new Litter { location_latitude = 50, location_longitude = 5 };

        _mockRepo.Setup(r => r.InsertAsync(It.IsAny<Litter>(), It.IsAny<Weather>()))
            .Callback<Litter, Weather>((l, w) =>
            {
                Assert.AreEqual(19, w.temperature_celsius);
                Assert.AreEqual(45, w.humidity);
                Assert.AreEqual("Sunny", w.conditions);
            })
            .ReturnsAsync(litter);

        //Act
        var result = await _controller.Add(litter);

        //Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    /// <summary>
    /// Tests that Add() returns 500 Internal Server Error if the Weather API fails.
    /// </summary>

    [TestMethod]
    public async Task Add_ReturnsServerError_WhenWeatherApiFails()
    {
        //Arrange
        var litter = new Litter { location_latitude = 50, location_longitude = 5 };

        var handler = new FakeHttpMessageHandler("{}", HttpStatusCode.InternalServerError);
        var httpClient = new HttpClient(handler);

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("fake");
        mockConfig.Setup(c => c.GetSection("WeatherApiKey")).Returns(mockSection.Object);

        var controller = new SensoringLitterController(
            _mockRepo.Object, _mockLogger.Object, mockFactory.Object, mockConfig.Object);

        //Act
        var result = await controller.Add(litter);

        //Assert
        var objResult = result as ObjectResult;
        Assert.IsNotNull(objResult);
        Assert.AreEqual(500, objResult.StatusCode);
    }
}

/// <summary>
/// A custom http message handler to use in the unit tests to fake the HTTP responses.
/// </summary>

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;
    private readonly HttpStatusCode _statusCode;

    public FakeHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
    {
        _responseContent = responseContent;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}

