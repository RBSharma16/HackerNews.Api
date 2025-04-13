using HackersNews.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace HackerNews.Test.Services
{
    public class HackersNewsApiClientFixture
    {
        public readonly int SampleNewsId = 43615912;
        public readonly List<StoryItems> SampleStoryItems;
        public readonly HttpResponseMessage SampleResponseOk = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"[ 43615912, 43613194, 43592116, 43602522, 43600363, 43615986, 43614199, 43585911, 43614285, 43615322, 43616604, 43617493, 43618140, 43618494, 43577490, 43618105, 43615925, 43612102, 43615346, 43616241 ]"),
        };
        public readonly HttpResponseMessage SampleItemResponseOk = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"{
                                            ""id"": 43615912,
                                            ""by"": ""PaulHoule"",
                                            ""type"": ""story"",
                                            ""title"": ""Middle-aged man trading cards go viral in rural Japan town"",
                                            ""url"": ""https://www.tokyoweekender.com/entertainment/middle-aged-man-trading-cards-go-viral-in-japan/""
                                          }"),
        };
        public readonly HttpResponseMessage SampleResponseNoContent = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent
        };

        public HackersNewsApiClientFixture() 
        {
            MockLogger = new Mock<ILogger<HackersNewsApiClient>>();
            MockHttpClientFactory = new Mock<IHttpClientFactory>();
            MockConfiguration = new Mock<IConfiguration>();
            MockMemoryCache = new Mock<IMemoryCache>();

            var mockSection = new Mock<IConfigurationSection>();            
            mockSection.Setup(x => x.Value).Returns("https://TestValue/");
            MockConfiguration.Setup(fu => fu.GetSection("AppSettings:HackerNews:BaseUrl")).Returns(mockSection.Object);

            mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("topstories");
            MockConfiguration.Setup(c => c.GetSection("AppSettings:HackerNews:TopStoriesPath")).Returns(mockSection.Object); 
            
            mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("item/{itemId}");
            MockConfiguration.Setup(c => c.GetSection("AppSettings:HackerNews:StoryItemsPath")).Returns(mockSection.Object);

            mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("HackerNewsData");
            MockConfiguration.Setup(c => c.GetSection("AppSettings:CacheName")).Returns(mockSection.Object);

            mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("1");
            MockConfiguration.Setup(c => c.GetSection("AppSettings:CacheExpiryMinuutes")).Returns(mockSection.Object);

            MockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(MockHttpMessageHandler.Object);
            MockHttpClientFactory.Setup(cf => cf.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var path = Directory.GetCurrentDirectory() + "/HackerNews.json";
            using (StreamReader r = new StreamReader(path)) 
            {
                SampleStoryItems = JsonConvert.DeserializeObject<List<StoryItems>>(r.ReadToEnd()) ?? new List<StoryItems>();
            }
        }

        public Mock<ILogger<HackersNewsApiClient>> MockLogger { get; set; }
        public Mock<IConfiguration> MockConfiguration { get; set; }
        public Mock<IMemoryCache> MockMemoryCache { get; set; }
        public Mock<IHttpClientFactory> MockHttpClientFactory { get; set; }
        public Mock<HttpMessageHandler> MockHttpMessageHandler { get; set; }

    }
}
