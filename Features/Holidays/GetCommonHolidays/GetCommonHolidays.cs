using HolidayApp.Database;
using HolidayApp.Features.Holidays.Shared;
using HolidayApp.Shared.Abstract;
using HolidayApp.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Features.Holidays.GetCommonHolidays;

public sealed record GetCommonHolidaysRequest(
    [FromQuery] string CountryCode1,
    [FromQuery] string CountryCode2,
    [FromQuery] int Year);

public sealed record GetCommonHolidaysResponse(DateOnly Date, List<LocalNamesDto> LocalNames);

public sealed record LocalNamesDto(string LocalName, string CountryCode);

public class GetCommonHolidays : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet(HolidaysApiEndpoints.CommonHolidays, Handle)
            .AddEndpointFilter<ValidationFilter<GetCommonHolidaysRequest>>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetCommonHolidaysRequest request,
        HolidaysDbContext dbContext,
        ILogger<GetCommonHolidays> logger,
        CancellationToken cancellationToken
    )
    {
        var holidays = await dbContext.Holiday
            .Where(h => h.Date.Year == request.Year &&
                        (h.CountryCode == request.CountryCode1 || h.CountryCode == request.CountryCode2))
            .Select(h => new { h.Date, h.LocalName, h.CountryCode })
            .ToListAsync(cancellationToken);

        var common = holidays
            .GroupBy(h => h.Date)
            .Where(g => g.Any(h => h.CountryCode == request.CountryCode1)
                        && g.Any(h => h.CountryCode == request.CountryCode2))
            .Select(g => new GetCommonHolidaysResponse(
                g.Key,
                g.DistinctBy(h => (h.LocalName, h.CountryCode))
                    .Select(h => new LocalNamesDto(h.LocalName, h.CountryCode))
                    .ToList()))
            .OrderBy(r => r.Date)
            .ToList();

        return Results.Ok(common);
    }
}