using SensoringApi.Controllers;
using SensoringApi.Interfaces;
using Moq;
using Microsoft.Extensions.Logging;

using SensoringApi.Models;
using Newtonsoft.Json;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System.Net;
using System.Net.Http;
using System.Text;

namespace SensoringApi.Test;

[TestClass]
public class SensoringLitterControllerTest
{
    private Mock<ILitterRepository> _mockRepo;
    private Mock<ILogger<SensoringLitterController>> _mockLogger;
    private SensoringLitterController _controller;

    [TestMethod]
    public async Task Add_ReturnsCreatedAt_WhenInputIsValid()
    {
        // Arrange
        var litter = new Litter { location_latitude = 50, location_longitude = 5 };

        var responseJson = JsonConvert.SerializeObject(new
        {
            current = new
            {
                temp_c = 18.2,
                humidity = 45,
                condition = new { text = "Sunny" }
            }
        });
        var handler = new FakeHttpMessageHandler(responseJson, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler);
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetValue<string>("WeatherApiKey")).Returns("fake");

        _mockRepo = new Mock<ILitterRepository>();
        _mockLogger = new Mock<ILogger<SensoringLitterController>>();
        _mockRepo.Setup(r => r.InsertAsync(It.IsAny<Litter>(), It.IsAny<Weather>()))
                 .ReturnsAsync(litter);

        _controller = new SensoringLitterController(
            _mockRepo.Object, _mockLogger.Object, mockFactory.Object, mockConfig.Object);
        var result = await _controller.Add(litter);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLatitudeAndLongitudeAreMissing()
    {
        var litter = new Litter
        {
            location_latitude = 0,
            location_longitude = 0
        };

        var result = await _controller.Add(litter);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLatitudeIsMissing()
    {
        var litter = new Litter
        {
            location_latitude = 0,
            location_longitude = 5
        };

        var result = await _controller.Add(litter);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Add_ReturnsBadRequest_WhenLongitudeIsMissing()
    {
        var litter = new Litter
        {
            location_latitude = 5,
            location_longitude = 0
        };

        var result = await _controller.Add(litter);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}


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

