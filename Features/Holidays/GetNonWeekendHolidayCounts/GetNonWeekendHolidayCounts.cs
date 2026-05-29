using HolidayApp.Database;
using HolidayApp.Features.Holidays.Shared;
using HolidayApp.Shared.Abstract;
using HolidayApp.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Features.Holidays.GetNonWeekendHolidayCounts;

public sealed record GetNonWeekendHolidayCountsRequest(
    [FromQuery] string[] CountryCodes,
    [FromQuery] int Year);

public sealed record GetNonWeekendHolidayCountsResponse(string CountryCode, int Count);

public class GetNonWeekendHolidayCounts : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet(HolidaysApiEndpoints.NonWeekendCount, Handle)
            .AddEndpointFilter<ValidationFilter<GetNonWeekendHolidayCountsRequest>>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetNonWeekendHolidayCountsRequest request,
        HolidaysDbContext dbContext,
        ILogger<GetNonWeekendHolidayCounts> logger,
        CancellationToken cancellationToken
    )
    {
        var codes = request.CountryCodes.ToHashSet();

        var holidays = await dbContext.Holiday
            .Where(h => h.Date.Year == request.Year
                        && codes.Contains(h.CountryCode)
                        && h.Date.DayOfWeek != DayOfWeek.Saturday
                        && h.Date.DayOfWeek != DayOfWeek.Sunday)
            .GroupBy(h => h.CountryCode)
            .Select(h => new GetNonWeekendHolidayCountsResponse(h.Key, h.Count()))
            .OrderByDescending(h => h.Count)
            .ToListAsync(cancellationToken);

        return Results.Ok(holidays);
    }
}