using HackersNews.Api.Controllers;
using HackersNews.Service;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace HackerNews.Test.Controllers
{
    public class HackerNewsControllerFixture
    {
        public readonly int SamplePageNumber = 1;
        public readonly int SamplePageSize = 2;
        public readonly string SampleSearchText = "Test";
        public List<StoryItems> SampleStoryItems = new List<StoryItems>();
        public List<int> SampleStoryIds = new List<int>() { 43615912, 43613194, 43592116, 43602522, 43600363, 43615986, 43614199, 43585911, 43614285, 43615322, 43616604, 43617493, 43618140, 43618494, 43577490, 43618105, 43615925, 43612102, 43615346, 43616241 };

        public HackerNewsControllerFixture()
        {
            var path = Directory.GetCurrentDirectory() + "/HackerNews.json";
            using (StreamReader r = new StreamReader(path))
                SampleStoryItems = JsonConvert.DeserializeObject<List<StoryItems>>(r.ReadToEnd()) ?? new List<StoryItems>();
            
            MockLogger = new Mock<ILogger<HackerNewsController>>();
            MockHackersNewsApiClient = new Mock<IHackersNewsApiClient>();
            MockHackersNewsApiClient.Setup(hn => hn.GetTopStoriesAsync()).ReturnsAsync(SampleStoryIds);
            MockHackersNewsApiClient.Setup(hn => hn.GetStoryItemAsync(It.IsAny<int>())).ReturnsAsync(SampleStoryItems.FirstOrDefault() ?? new StoryItems());
        }
        public Mock<ILogger<HackerNewsController>> MockLogger { get; set; }
        public Mock<IHackersNewsApiClient> MockHackersNewsApiClient { get; set; }

    }
}
