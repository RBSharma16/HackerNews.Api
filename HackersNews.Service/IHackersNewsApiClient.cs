namespace HackersNews.Service
{
    /// <summary>
    /// Hacker news API client.
    /// </summary>
    public interface IHackersNewsApiClient
    {
        /// <summary>
        /// Get list of all the top story id's.
        /// </summary>
        /// <returns></returns>
        Task<List<int>> GetTopStoriesAsync();

        /// <summary>
        /// Get details of story item by the id.
        /// </summary>
        /// <param name="storyItemId">Story item id</param>
        /// <returns></returns>
        Task<StoryItems> GetStoryItemAsync(int storyItemId);

        /// <summary>
        /// Get cached data if available
        /// </summary>
        /// <returns></returns>
        Task<List<StoryItems>> GetStoryItemsAsync();
    }
}
