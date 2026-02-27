using Umbraco13.Models;

namespace Umbraco13.Services;

/// <summary>
/// Interface for Media API operations using Umbraco Content Delivery API
/// </summary>
public interface IMediaApiService
{
    /// <summary>
    /// Gets media items by name from a specific media item
    /// </summary>
    /// <param name="mediaItemPath">Path to the parent media item (e.g., "folder/subfolder")</param>
    /// <param name="name">Name of the media item to search for</param>
    /// <param name="mediaType">Optional filter by media type (e.g., "Image", "File")</param>
    /// <returns>List of media items matching the criteria</returns>
    Task<List<DeliveryApiContentItem>> GetMediaItemsByNameAsync(string mediaItemPath, string name, string? mediaType = null);

    /// <summary>
    /// Gets a single media item by its GUID
    /// </summary>
    /// <param name="id">GUID of the media item</param>
    /// <returns>Media item or null if not found</returns>
    Task<DeliveryApiContentItem?> GetMediaItemByIdAsync(Guid id);
}
