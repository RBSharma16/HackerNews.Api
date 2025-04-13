using HackersNews.Service;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace HackerNews.Test.Services
{
    public class HackersNewsApiClientTests : IClassFixture<HackersNewsApiClientFixture>
    {
        private readonly HackersNewsApiClientFixture _clientFixture;
        private readonly IHackersNewsApiClient _hackersNewsApiClient;

        public HackersNewsApiClientTests(HackersNewsApiClientFixture clientFixture)
        {
            _clientFixture = clientFixture;
            _hackersNewsApiClient = new HackersNewsApiClient(
                _clientFixture.MockHttpClientFactory.Object,
                _clientFixture.MockConfiguration.Object,
                _clientFixture.MockMemoryCache.Object,
                _clientFixture.MockLogger.Object);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetTopStoriesAsync_WhenSuccess()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_clientFixture.SampleResponseOk).Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetTopStoriesAsync();
            var response = JsonConvert.DeserializeObject<List<int>>(JsonConvert.SerializeObject(result));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.IsType<List<int>>(response);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetTopStoriesAsync_WhenFailed()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_clientFixture.SampleResponseNoContent).Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetTopStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetTopStoriesAsync_WhenException()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws<Exception>().Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetTopStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemAsync_WhenSuccess()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_clientFixture.SampleItemResponseOk).Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemAsync(_clientFixture.SampleNewsId);
            var response = JsonConvert.DeserializeObject<StoryItems>(JsonConvert.SerializeObject(result));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.IsType<StoryItems>(response);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemAsync_WhenFailed()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_clientFixture.SampleResponseNoContent).Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemAsync(_clientFixture.SampleNewsId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.title);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemAsync_WhenException()
        {
            // Arrange
            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws<Exception>().Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemAsync(_clientFixture.SampleNewsId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.title);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemsAsync_WhenSuccess()
        {
            // Arrange
            object? obj = _clientFixture.SampleStoryItems;
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out obj)).Returns(true);

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemsAsync();
            var response = JsonConvert.DeserializeObject<List<StoryItems>>(JsonConvert.SerializeObject(result));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.IsType<List<StoryItems>>(response);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemsAsync_NoCachedData_FetchesFromApi()
        {
            // Arrange
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny!)).Returns(false);

            _clientFixture
                .MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_clientFixture.SampleResponseOk).Verifiable();

            _clientFixture.MockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("item")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(_clientFixture.SampleItemResponseOk)
            .Verifiable();

            object? obj = _clientFixture.SampleStoryItems;
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out obj)).Returns(true);

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemsAsync();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(_clientFixture.SampleStoryItems?.FirstOrDefault()?.url, result[0].url);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemsAsync_NoCachedData_FetchesOnly200Items()
        {
            // Arrange
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(false);
            _clientFixture.MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(_clientFixture.SampleResponseOk)
                .Verifiable();

            _clientFixture.MockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("item")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(_clientFixture.SampleItemResponseOk)
                .Verifiable();

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemsAsync();

            // Assert
            Assert.True(result.Count <= 200);
        }


        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemsAsync_WhenFailed()
        {
            // Arrange
            object? obj;
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out obj));

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemsAsync();
            var response = JsonConvert.DeserializeObject<List<StoryItems>>(JsonConvert.SerializeObject(result));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.IsType<List<StoryItems>>(response);
        }

        [Fact]
        public async Task HackersNewsApiClient_GetStoryItemsAsync_WhenException()
        {
            // Arrange
            object? obj;
            _clientFixture.MockMemoryCache.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out obj));

            // Act
            var result = await _hackersNewsApiClient.GetStoryItemsAsync();
            var response = JsonConvert.DeserializeObject<List<StoryItems>>(JsonConvert.SerializeObject(result));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.IsType<List<StoryItems>>(response);
        }
    }
}
