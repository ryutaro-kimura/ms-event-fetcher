namespace MsEventFetcher.Models;

using System.Text.Json.Serialization;

public sealed class MsEventsResponse
{
    [JsonPropertyName("cards")]
    public List<MsEventCard> Cards { get; set; } = [];
}

public sealed class MsEventCard
{
    [JsonPropertyName("content")]
    public MsEventContent Content { get; set; } = new();
}

public sealed class MsEventContent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("formatEnglishName")]
    public string FormatEnglishName { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public MsEventAction Action { get; set; } = new();

    [JsonPropertyName("location")]
    public MsEventLocation Location { get; set; } = new();

    [JsonPropertyName("eventDates")]
    public MsEventDates EventDates { get; set; } = new();
}

public sealed class MsEventAction
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}

public sealed class MsEventLocation
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public sealed class MsEventDates
{
    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTimeOffset? EndDate { get; set; }
}
