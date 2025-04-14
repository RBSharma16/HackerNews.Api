using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HackersNews.Service
{
    /// <summary>
    /// Hacker news API client.
    /// </summary>
    public class HackersNewsApiClient : IHackersNewsApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HackersNewsApiClient> _logger;
        private readonly string _baseUrl;
        private readonly string _topStoriesPath;
        private readonly string _storyItemsPath;
        private readonly string _cacheName;
        private readonly int _cacheExpiryMinutes;

        /// <summary>
        /// Constructor of hacker news API client.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="configuration"></param>
        public HackersNewsApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache, ILogger<HackersNewsApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
            _baseUrl = configuration.GetValue<string>("AppSettings:HackerNews:BaseUrl") ?? string.Empty;
            _topStoriesPath = configuration.GetValue<string>("AppSettings:HackerNews:TopStoriesPath") ?? string.Empty;
            _storyItemsPath = configuration.GetValue<string>("AppSettings:HackerNews:StoryItemsPath") ?? string.Empty;
            _cacheName = configuration.GetValue<string>("AppSettings:CacheName") ?? string.Empty;
            _cacheExpiryMinutes = configuration.GetValue<int>("AppSettings:CacheExpiryMinuutes");
        }

        /// <summary>
        /// Get list of all the top story id's.
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetTopStoriesAsync()
        {
            var ids = new List<int>();
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(_baseUrl + _topStoriesPath, UriKind.Absolute)
                    };

                    using (var httpResponseMessage = await httpClient.SendAsync(request))
                    {
                        var response = await httpResponseMessage.Content.ReadAsStringAsync();

                        if (httpResponseMessage.IsSuccessStatusCode)
                            return JsonConvert.DeserializeObject<List<int>>(response) ?? ids;
                        else
                            return ids;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ids;
            }
        }

        /// <summary>
        /// Get details of story item by the id.
        /// </summary>
        /// <param name="storyItemId">Story item id</param>
        /// <returns></returns>
        public async Task<StoryItems> GetStoryItemAsync(int storyItemId)
        {
            var storyItems = new StoryItems();
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(_baseUrl + _storyItemsPath.Replace("{itemId}", storyItemId.ToString()), UriKind.Absolute)
                    };

                    using (var httpResponseMessage = await httpClient.SendAsync(request))
                    {
                        var response = await httpResponseMessage.Content.ReadAsStringAsync();

                        if (httpResponseMessage.IsSuccessStatusCode)
                            return JsonConvert.DeserializeObject<StoryItems>(response) ?? storyItems;
                        else
                            return storyItems;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return storyItems;
            }
        }

        /// <summary>
        /// Get cached data if available
        /// </summary>
        /// <returns></returns>
        public async Task<List<StoryItems>> GetStoryItemsAsync()
        {
            try
            {
                if (!_memoryCache.TryGetValue(_cacheName, out List<StoryItems>? storyItems))
                {
                    var ids = await GetTopStoriesAsync();

                    var tasks = ids.Select(async id =>
                    {
                        var item = await GetStoryItemAsync(id);
                        return item;
                    });

                    var items = await Task.WhenAll(tasks);
                    storyItems = items.Where(item => item != null && !string.IsNullOrEmpty(item.url)).Take(200).ToList();

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpiryMinutes)
                    };
                    _memoryCache.Set(_cacheName, storyItems, memoryCacheEntryOptions);
                }
                return storyItems ?? new List<StoryItems>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<StoryItems>();
            }
        }
    }
}
