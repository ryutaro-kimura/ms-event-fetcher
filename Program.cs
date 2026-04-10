using System.Text.Json;
using MsEventFetcher.Services;

// 検索キーワード（Microsoft 関連）
var keywords = new[]
{
    "Microsoft",
    "Azure",
    "Microsoft 365",
    ".NET",
    "GitHub Copilot"
};

Console.WriteLine("[INFO] Fetching Microsoft-related events from connpass...");

using var connpass = new ConnpassClient();
var events = await connpass.FetchEventsAsync(keywords);

Console.WriteLine($"[INFO] Found {events.Count} upcoming events.");

// コンソールにサマリ出力
foreach (var ev in events)
{
    var startDate = DateTime.TryParse(ev.StartedAt, out var dt)
        ? dt.ToString("yyyy/MM/dd (ddd) HH:mm")
        : ev.StartedAt;

    Console.WriteLine($"  {startDate} | {ev.Title}");
    Console.WriteLine($"    URL: {ev.EventUrl}");
    Console.WriteLine($"    参加者: {ev.Accepted}" + (ev.Limit.HasValue ? $"/{ev.Limit}" : ""));
    Console.WriteLine();
}

// JSON ファイルに出力（GitHub Actions の Artifacts 用）
var outputPath = Environment.GetEnvironmentVariable("EVENT_OUTPUT_PATH") ?? "events.json";
var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
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
