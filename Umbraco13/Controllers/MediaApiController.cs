using Microsoft.AspNetCore.Mvc;
using Umbraco13.Services;
using Microsoft.AspNetCore.Authorization;


namespace Umbraco13.Controllers;

/// <summary>
/// Controller for querying media items using Umbraco Content Delivery API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MediaApiController : Controller
{
    private readonly IMediaApiService _mediaApiService;
    private readonly ILogger<MediaApiController> _logger;

    public MediaApiController(IMediaApiService mediaApiService, ILogger<MediaApiController> logger)
    {
        _mediaApiService = mediaApiService;
        _logger = logger;
    }

    /// <summary>
    /// Get media items by name from a specific media item (folder)
    /// </summary>
    /// <param name="mediaItemPath">Path to the parent media item (e.g., "folder/subfolder"). Use "/" for root.</param>
    /// <param name="name">Name of the media item to search for (supports partial matching)</param>
    /// <param name="mediaType">Optional filter by media type (e.g., "Image", "File", "Folder")</param>
    /// <returns>List of media items matching the criteria</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchMediaByName(
        [FromQuery] string mediaItemPath = "/",
        [FromQuery] string? name = null,
        [FromQuery] string? mediaType = null)
    {
        try
        {
            _logger.LogInformation("SearchMediaByName called with Path: {Path}, Name: {Name}, MediaType: {Type}",
                mediaItemPath, name, mediaType);

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("SearchMediaByName called without name parameter");
                return BadRequest(new
                {
                    error = "Name parameter is required",
                    message = "Please provide a name to search for"
                });
            }

            var mediaItems = await _mediaApiService.GetMediaItemsByNameAsync(mediaItemPath, name, mediaType);

            return Ok(new
            {
                success = true,
                count = mediaItems.Count,
                items = mediaItems
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while searching media items");
            return StatusCode(503, new
            {
                error = "Service unavailable",
                message = "Unable to connect to the Media Delivery API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching media items");
            return StatusCode(500, new
            {
                error = "Internal server error",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get a media item by its GUID
    /// </summary>
    /// <param name="id">GUID of the media item</param>
    /// <returns>Media item details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMediaById(Guid id)
    {
        try
        {
            _logger.LogInformation("GetMediaById called with ID: {Id}", id);

            var mediaItem = await _mediaApiService.GetMediaItemByIdAsync(id);

            if (mediaItem == null)
            {
                _logger.LogWarning("Media item not found with ID: {Id}", id);
                return NotFound(new
                {
                    error = "Media item not found",
                    message = $"No media item found with ID: {id}"
                });
            }

            return Ok(new
            {
                success = true,
                item = mediaItem
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching media item {Id}", id);
            return StatusCode(503, new
            {
                error = "Service unavailable",
                message = "Unable to connect to the Media Delivery API"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching media item {Id}", id);
            return StatusCode(500, new
            {
                error = "Internal server error",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the Media API
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "MediaApiController"
        });
    }
}
