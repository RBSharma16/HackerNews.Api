using HackersNews.Api.Controllers;
using HackersNews.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace HackerNews.Test.Controllers
{
    public class HackerNewsControllerTests : IClassFixture<HackerNewsControllerFixture>
    {
        private readonly Mock<IHackersNewsApiClient> _mockHackersNewsApiClient;
        private readonly HackerNewsController _controller;
        private readonly HackerNewsControllerFixture _hackerNewsControllerFixture;

        public HackerNewsControllerTests(HackerNewsControllerFixture hackerNewsControllerFixture)
        {
            _hackerNewsControllerFixture = hackerNewsControllerFixture;
            _mockHackersNewsApiClient = _hackerNewsControllerFixture.MockHackersNewsApiClient;
            _controller = new HackerNewsController(_hackerNewsControllerFixture.MockHackersNewsApiClient.Object, _hackerNewsControllerFixture.MockLogger.Object);
        }

        [Fact]
        public async Task HackerNewsController_GetStoryItems_BadRequest_When_EmptyItems()
        {
            // Arrange
            _mockHackersNewsApiClient.Setup(hn => hn.GetStoryItemsAsync()).ReturnsAsync(new List<StoryItems>());

            // Act
            var result = await _controller.GetStoryItems(
                _hackerNewsControllerFixture.SamplePageNumber, 
                _hackerNewsControllerFixture.SamplePageSize
                ) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Item not found.", result.Value);
        }

        [Fact]
        public async Task HackerNewsController_GetStoryItems_BadRequest_When_Exception()
        {
            // Arrange
            _mockHackersNewsApiClient.Setup(hn => hn.GetStoryItemsAsync()).Throws<Exception>();

            // Act
            var result = await _controller.GetStoryItems(
                _hackerNewsControllerFixture.SamplePageNumber,
                _hackerNewsControllerFixture.SamplePageSize
                ) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("Internal server error.", result.Value);
        }

        [Fact]
        public async Task HackerNewsController_GetStoryItems_Success_When_NoItemsAfterFilter()
        {
            // Arrange
            _mockHackersNewsApiClient.Setup(hn => hn.GetStoryItemsAsync()).ReturnsAsync(_hackerNewsControllerFixture.SampleStoryItems?.Take(1).ToList());

            // Act
            var result = await _controller.GetStoryItems(
                _hackerNewsControllerFixture.SamplePageNumber,
                _hackerNewsControllerFixture.SamplePageSize,
                _hackerNewsControllerFixture.SampleStoryItems?.Where(x => x.id == 43615912).FirstOrDefault()?.title
                ) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task HackerNewsController_GetStoryItems_Success()
        {
            // Arrange
            _mockHackersNewsApiClient.Setup(hn => hn.GetStoryItemsAsync()).ReturnsAsync(_hackerNewsControllerFixture.SampleStoryItems);

            // Act
            var result = await _controller.GetStoryItems(
                _hackerNewsControllerFixture.SamplePageNumber,
                _hackerNewsControllerFixture.SamplePageSize
                ) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Value);
        }
    }
}
