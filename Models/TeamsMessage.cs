namespace MsEventFetcher.Models;

using System.Text.Json.Serialization;

public sealed class TeamsMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "message";

    [JsonPropertyName("attachments")]
    public List<TeamsAttachment> Attachments { get; set; } = [];
}

public sealed class TeamsAttachment
{
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = "application/vnd.microsoft.card.adaptive";

    [JsonPropertyName("contentUrl")]
    public string? ContentUrl { get; set; }

    [JsonPropertyName("content")]
    public AdaptiveCard Content { get; set; } = new();
}

public sealed class AdaptiveCard
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "http://adaptivecards.io/schemas/adaptive-card.json";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "AdaptiveCard";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.4";

    [JsonPropertyName("body")]
    public List<object> Body { get; set; } = [];
}
