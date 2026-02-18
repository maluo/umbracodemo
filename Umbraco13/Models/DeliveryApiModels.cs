using System.Text.Json.Serialization;

namespace Umbraco13.Models;

public class DeliveryApiPagedResponse<T>
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}

public class DeliveryApiContentItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("route")]
    public DeliveryApiRoute Route { get; set; } = new();

    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class DeliveryApiRoute
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("startItem")]
    public DeliveryApiStartItem StartItem { get; set; } = new();
}

public class DeliveryApiStartItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;
}
