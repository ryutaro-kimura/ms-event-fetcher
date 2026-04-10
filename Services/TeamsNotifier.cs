namespace MsEventFetcher.Services;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MsEventFetcher.Models;

public sealed class TeamsNotifier : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string? _webhookUrl;

    public TeamsNotifier(string? webhookUrl)
    {
        _webhookUrl = webhookUrl;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Teams Incoming Webhook にイベント一覧を通知する
    /// </summary>
    public async Task<bool> NotifyAsync(List<ConnpassEvent> events)
    {
        if (string.IsNullOrWhiteSpace(_webhookUrl))
        {
            Console.WriteLine("[INFO] TEAMS_WEBHOOK_URL is not set. Skipping Teams notification.");
            return false;
        }

        if (events.Count == 0)
        {
            Console.WriteLine("[INFO] No events to notify.");
            return false;
        }

        var message = BuildAdaptiveCardMessage(events);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        var response = await _httpClient.PostAsJsonAsync(_webhookUrl, message, options);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"[ERROR] Teams notification failed ({response.StatusCode}): {body}");
            return false;
        }

        Console.WriteLine($"[INFO] Teams notification sent. {events.Count} events.");
        return true;
    }

    private static TeamsMessage BuildAdaptiveCardMessage(List<ConnpassEvent> events)
    {
        var body = new List<object>
        {
            new Dictionary<string, object>
            {
                ["type"] = "TextBlock",
                ["size"] = "Large",
                ["weight"] = "Bolder",
                ["text"] = $"📅 Microsoft 関連イベント ({events.Count}件)",
                ["wrap"] = true
            },
            new Dictionary<string, object>
            {
                ["type"] = "TextBlock",
                ["text"] = $"取得日時: {DateTime.UtcNow.AddHours(9):yyyy/MM/dd HH:mm} (JST)",
                ["isSubtle"] = true,
                ["wrap"] = true
            }
        };

        // 最大20件に制限（Adaptive Card のサイズ上限対策）
        foreach (var ev in events.Take(20))
        {
            var startDate = DateTime.TryParse(ev.StartedAt, out var dt)
                ? dt.ToString("yyyy/MM/dd (ddd) HH:mm")
                : ev.StartedAt;

            body.Add(new Dictionary<string, object>
            {
                ["type"] = "Container",
                ["separator"] = true,
                ["items"] = new List<object>
                {
                    new Dictionary<string, object>
                    {
                        ["type"] = "TextBlock",
                        ["text"] = $"**{ev.Title}**",
                        ["wrap"] = true
                    },
                    new Dictionary<string, object>
                    {
                        ["type"] = "ColumnSet",
                        ["columns"] = new List<object>
                        {
                            new Dictionary<string, object>
                            {
                                ["type"] = "Column",
                                ["width"] = "auto",
                                ["items"] = new List<object>
                                {
                                    new Dictionary<string, object>
                                    {
                                        ["type"] = "TextBlock",
                                        ["text"] = $"🕐 {startDate}",
                                        ["isSubtle"] = true,
                                        ["wrap"] = true
                                    }
                                }
                            },
                            new Dictionary<string, object>
                            {
                                ["type"] = "Column",
                                ["width"] = "auto",
                                ["items"] = new List<object>
                                {
                                    new Dictionary<string, object>
                                    {
                                        ["type"] = "TextBlock",
                                        ["text"] = $"👥 {ev.Accepted}" + (ev.Limit.HasValue ? $"/{ev.Limit}" : ""),
                                        ["isSubtle"] = true,
                                        ["wrap"] = true
                                    }
                                }
                            }
                        }
                    },
                    new Dictionary<string, object>
                    {
                        ["type"] = "ActionSet",
                        ["actions"] = new List<object>
                        {
                            new Dictionary<string, object>
                            {
                                ["type"] = "Action.OpenUrl",
                                ["title"] = "詳細を見る",
                                ["url"] = ev.EventUrl
                            }
                        }
                    }
                }
            });
        }

        if (events.Count > 20)
        {
            body.Add(new Dictionary<string, object>
            {
                ["type"] = "TextBlock",
                ["text"] = $"※ 他 {events.Count - 20}件のイベントがあります",
                ["isSubtle"] = true,
                ["wrap"] = true
            });
        }

        return new TeamsMessage
        {
            Attachments =
            [
                new TeamsAttachment
                {
                    Content = new AdaptiveCard { Body = body }
                }
            ]
        };
    }

    public void Dispose() => _httpClient.Dispose();
}
