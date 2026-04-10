namespace MsEventFetcher.Services;

using System.Net.Http.Json;
using System.Text.Json;
using MsEventFetcher.Models;

public sealed class ConnpassClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://connpass.com/api/v1/event/";

    public ConnpassClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MsEventFetcher/1.0");
    }

    /// <summary>
    /// connpass API からキーワードに一致するイベントを取得する
    /// </summary>
    public async Task<List<ConnpassEvent>> FetchEventsAsync(string[] keywords, int count = 30)
    {
        var allEvents = new Dictionary<int, ConnpassEvent>();

        foreach (var keyword in keywords)
        {
            var url = $"{BaseUrl}?keyword={Uri.EscapeDataString(keyword)}&count={count}&order=2";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ConnpassResponse>(url);
                if (response?.Events is null) continue;

                foreach (var ev in response.Events)
                {
                    // 未来のイベントのみ、重複排除
                    if (DateTime.TryParse(ev.StartedAt, out var startDate) && startDate > DateTime.UtcNow)
                    {
                        allEvents.TryAdd(ev.EventId, ev);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"[WARN] Failed to fetch events for keyword '{keyword}': {ex.Message}");
            }

            // connpass API のレートリミット対策
            await Task.Delay(1000);
        }

        return allEvents.Values
            .OrderBy(e => e.StartedAt)
            .ToList();
    }

    public void Dispose() => _httpClient.Dispose();
}
