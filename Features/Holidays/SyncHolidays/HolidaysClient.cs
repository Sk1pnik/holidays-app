using System.Net;
using System.Text.Json.Serialization;

namespace HolidayApp.Features.Holidays.SyncHolidays;

public sealed record HolidayClientResponse(
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("localName")] string LocalName,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("countryCode")] string CountryCode);

public interface IHolidaysClient
{
    public Task<IEnumerable<HolidayClientResponse>?> GetHolidaysAsync(int year, string countryCode,
        CancellationToken cancellationToken);
}

public class HolidayClient(
    ILogger<HolidayClient> logger,
    HttpClient httpClient
) : IHolidaysClient
{
    public async Task<IEnumerable<HolidayClientResponse>?> GetHolidaysAsync(int year, string countryCode,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching public holidays for {CountryCode} / {Year}.", countryCode, year);

        var response = await httpClient.GetAsync($"{year}/{countryCode}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("No holidays found for {CountryCode}/{Year}.", countryCode, year);
            return null;
        }

        response.EnsureSuccessStatusCode();

        var holidays =
            await response.Content.ReadFromJsonAsync<List<HolidayClientResponse>>(cancellationToken: cancellationToken);

        var count = holidays?.Count ?? 0;

        logger.LogInformation(
            "Retrieved {Count} holidays for {CountryCode} in {Year}.",
            count, countryCode, year);

        return holidays;
    }
}