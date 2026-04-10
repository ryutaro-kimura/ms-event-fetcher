namespace MsEventFetcher.Services;

using System.Net.Http.Json;
using MsEventFetcher.Models;

public sealed class MsEventsClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.microsoft.com/msonecloudapi/events/cards";

    public MsEventsClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MsEventFetcher/1.0");
    }

    /// <summary>
    /// Microsoft 公式サイトのイベントカタログ API から日本語イベントを取得する
    /// </summary>
    public async Task<List<MsEventContent>> FetchEventsAsync(string filter = "primary-language:japanese")
    {
        var url = string.IsNullOrEmpty(filter)
            ? $"{BaseUrl}?onload=true&locale=ja-jp"
            : $"{BaseUrl}?for={Uri.EscapeDataString(filter)}&locale=ja-jp";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<MsEventsResponse>(url);
            if (response?.Cards is null || response.Cards.Count == 0)
            {
                Console.WriteLine("[WARN] No events returned from Microsoft Events API.");
                return [];
            }

            return response.Cards
                .Select(c => c.Content)
                .Where(e => e.EventDates.StartDate.HasValue && e.EventDates.StartDate > DateTimeOffset.UtcNow)
                .OrderBy(e => e.EventDates.StartDate)
                .ToList();
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"[ERROR] Failed to fetch events from Microsoft: {ex.Message}");
            return [];
        }
    }

    public void Dispose() => _httpClient.Dispose();
}
