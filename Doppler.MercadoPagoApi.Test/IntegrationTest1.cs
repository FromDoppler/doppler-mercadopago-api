using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Doppler.HelloMicroservice
{
    public class IntegrationTest1
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTest1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/WeatherForecast", HttpStatusCode.OK, "application/json; charset=utf-8")]
        [InlineData("/swagger", HttpStatusCode.Moved, null)]
        [InlineData("/swagger/index.html", HttpStatusCode.OK, "text/html; charset=utf-8")]
        [InlineData("/robots.txt", HttpStatusCode.OK, "text/plain")]
        [InlineData("/favicon.ico", HttpStatusCode.OK, "image/x-icon")]
        [InlineData("/swagger/v1/swagger.json", HttpStatusCode.OK, "application/json; charset=utf-8")]
        [InlineData("/", HttpStatusCode.NotFound, "application/problem+json; charset=utf-8")]
        [InlineData("/Not/Found", HttpStatusCode.NotFound, "application/problem+json; charset=utf-8")]
        public async Task GET_endpoints_return_correct_status_and_contentType(string url, HttpStatusCode expectedStatusCode, string expectedContentType)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedContentType, response.Content?.Headers?.ContentType?.ToString());
        }

        [Fact]
        public async Task GET_WeatherForecast_should_return_problem_details_when_there_is_an_unexpected_exception()
        {
            // Arrange
            var exceptionMessage = "Test unexpected exception";
            var weatherForecastServiceMock = new Mock<Weather.IWeatherForecastService>();
            weatherForecastServiceMock.Setup(x => x.GetForecasts())
                .Throws(new Exception(exceptionMessage));

            var client = _factory
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureServices(s =>
                    {
                        s.AddSingleton(weatherForecastServiceMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions()
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.GetAsync("/WeatherForecast");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("application/problem+json; charset=utf-8", response.Content?.Headers?.ContentType?.ToString());
            Assert.Contains($"\"detail\":\"{exceptionMessage}\"", responseContent);
            Assert.Contains("\"title\":\"Internal Server Error\"", responseContent);
            Assert.Contains("\"type\":\"https://httpstatuses.com/500\"", responseContent);
            Assert.Contains("\"status\":500", responseContent);
            Assert.Contains("\"type\":\"System.Exception\"", responseContent);
            Assert.Contains($"\"raw\":\"System.Exception: {exceptionMessage}", responseContent);
        }

    }
}
