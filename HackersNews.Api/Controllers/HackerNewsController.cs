using HackersNews.Service;
using Microsoft.AspNetCore.Mvc;

namespace HackersNews.Api.Controllers
{
    /// <summary>
    /// Hacker news controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackersNewsApiClient _hackersNewsApiClient;
        private readonly ILogger<HackerNewsController> _logger;

        /// <summary>
        /// Controller constructor.
        /// </summary>
        /// <param name="hackersNewsApiClient"></param>
        /// <param name="logger"></param>
        public HackerNewsController(IHackersNewsApiClient hackersNewsApiClient, ILogger<HackerNewsController> logger) 
        {
            _hackersNewsApiClient = hackersNewsApiClient;
            _logger = logger;
        }

        /// <summary>
        /// Get story items by pageno, search criteria
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpGet("{pageNumber}/{pageSize}/items")]
        public async Task<IActionResult> GetStoryItems(int pageNumber, int pageSize, string? title = null)
        {
            try
            {
                var items = await _hackersNewsApiClient.GetStoryItemsAsync();
                if (items != null && items.Count > 0)
                {
                    items = items.Where(x => string.IsNullOrEmpty(title) || (x.title ?? string.Empty).Contains(title, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    var total = items.Count;
                    if (items.Any() && items.Count >= (pageNumber*pageSize))
                        items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    return Ok(new { items, total});
                }
                else
                {
                    return BadRequest("Item not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
