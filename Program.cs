using System.Text.Encodings.Web;
using System.Text.Json;
using MsEventFetcher.Services;

Console.WriteLine("[INFO] Fetching Microsoft events from official catalog...");

using var client = new MsEventsClient();
var events = await client.FetchEventsAsync();

Console.WriteLine($"[INFO] Found {events.Count} upcoming events.");

// コンソールにサマリ出力
foreach (var ev in events)
{
    var startDate = ev.EventDates.StartDate?.ToOffset(TimeSpan.FromHours(9)).ToString("yyyy/MM/dd (ddd) HH:mm") ?? "未定";
    var location = ev.Format;
    if (!string.IsNullOrEmpty(ev.Location.City) && ev.Location.City != "Digital")
    {
        location += $" / {ev.Location.City}";
    }

    Console.WriteLine($"  {startDate} | {ev.Title}");
    Console.WriteLine($"    URL: {ev.Action.Href}");
    Console.WriteLine($"    形式: {location}");
    Console.WriteLine();
}

// JSON ファイルに出力（GitHub Actions の Artifacts 用）
var outputPath = Environment.GetEnvironmentVariable("EVENT_OUTPUT_PATH") ?? "events.json";
var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
await File.WriteAllTextAsync(outputPath, json);
Console.WriteLine($"[INFO] Events saved to {outputPath}");

// Teams 通知
var webhookUrl = Environment.GetEnvironmentVariable("TEAMS_WEBHOOK_URL");
using var notifier = new TeamsNotifier(webhookUrl);
await notifier.NotifyAsync(events);

// GitHub Actions の出力として件数を設定
var githubOutput = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
if (!string.IsNullOrEmpty(githubOutput))
{
    await File.AppendAllTextAsync(githubOutput, $"event_count={events.Count}\n");
}
