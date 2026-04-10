namespace MsEventFetcher.Models;

using System.Text.Json.Serialization;

public sealed class ConnpassResponse
{
    [JsonPropertyName("results_returned")]
    public int ResultsReturned { get; set; }

    [JsonPropertyName("results_available")]
    public int ResultsAvailable { get; set; }

    [JsonPropertyName("events")]
    public List<ConnpassEvent> Events { get; set; } = [];
}

public sealed class ConnpassEvent
{
    [JsonPropertyName("event_id")]
    public int EventId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("catch")]
    public string Catch { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("event_url")]
    public string EventUrl { get; set; } = string.Empty;

    [JsonPropertyName("started_at")]
    public string StartedAt { get; set; } = string.Empty;

    [JsonPropertyName("ended_at")]
    public string EndedAt { get; set; } = string.Empty;

    [JsonPropertyName("place")]
    public string Place { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("accepted")]
    public int Accepted { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = string.Empty;

    [JsonPropertyName("owner_display_name")]
    public string OwnerDisplayName { get; set; } = string.Empty;
}
