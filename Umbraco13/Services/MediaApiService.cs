using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbraco13.Models;

namespace Umbraco13.Services;

/// <summary>
/// Service for interacting with Umbraco Content Delivery API for media items
/// </summary>
public class MediaApiService : IMediaApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MediaApiService> _logger;
    private readonly string _deliveryApiUrl;
    private readonly string _apiKey;

    public MediaApiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MediaApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        _deliveryApiUrl = configuration["Umbraco:CMS:DeliveryApi:DeliveryApiUrl"]
            ?? throw new InvalidOperationException("Umbraco:CMS:DeliveryApi:DeliveryApiUrl is not configured");

        _apiKey = configuration["Umbraco:CMS:DeliveryApi:ApiKey"]
            ?? throw new InvalidOperationException("Umbraco:CMS:DeliveryApi:ApiKey is not configured");

        _logger.LogInformation("MediaApiService initialized with DeliveryApiUrl: {Url}", _deliveryApiUrl);
    }

    public async Task<List<DeliveryApiContentItem>> GetMediaItemsByNameAsync(string mediaItemPath, string name, string? mediaType = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Api-Key", _apiKey);

            // Build query parameters
            var queryParams = new List<string>
            {
                $"fetch=children:{mediaItemPath}",
                $"filter=name:{name}"
            };

            // Add media type filter if specified
            if (!string.IsNullOrEmpty(mediaType))
            {
                queryParams.Add($"filter=mediaType:{mediaType}");
            }

            // Build request URL
            var query = string.Join("&", queryParams);
            var requestUrl = $"{_deliveryApiUrl.TrimEnd('/')}/media?{query}";

            _logger.LogInformation("Requesting media items from: {Url}", requestUrl);

            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Deserialize the response - Delivery API returns a paged response
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<DeliveryApiPagedResponse<DeliveryApiContentItem>>(content, options);

            if (apiResponse?.Items == null)
            {
                _logger.LogWarning("No items found in response");
                return new List<DeliveryApiContentItem>();
            }

            _logger.LogInformation($"Found {apiResponse.Items.Count()} media items matching name '{name}'");
            return apiResponse.Items.ToList();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching media items by name");
            return new List<DeliveryApiContentItem>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while parsing media response");
            return new List<DeliveryApiContentItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching media items by name");
            return new List<DeliveryApiContentItem>();
        }
    }

    public async Task<DeliveryApiContentItem?> GetMediaItemByIdAsync(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Api-Key", _apiKey);

            var requestUrl = $"{_deliveryApiUrl.TrimEnd('/')}/media/item/{id}";

            _logger.LogInformation("Requesting media item by ID: {Id}", id);

            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var mediaItem = JsonSerializer.Deserialize<DeliveryApiContentItem>(content, options);

            _logger.LogInformation("Successfully retrieved media item: {Id}, Name: {Name}",
                mediaItem?.Id, mediaItem?.Name);

            return mediaItem;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching media item by ID {Id}", id);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while parsing media response for ID {Id}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching media item by ID {Id}", id);
            return null;
        }
    }
}
