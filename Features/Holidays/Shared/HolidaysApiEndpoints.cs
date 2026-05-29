namespace HolidayApp.Features.Holidays.Shared;

public static class HolidaysApiEndpoints
{
    private const string ApiBase = "api";
    private const string Base = $"{ApiBase}/holidays";

    public const string Sync = $"{Base}/sync";
    public const string LastCelebrated = $"{Base}/{{countryCode}}/last-celebrated";
    public const string NonWeekendCount = $"{Base}/non-weekend-count";
    public const string CommonHolidays = $"{Base}/common";
}