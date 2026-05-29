using HolidayApp.Database;
using HolidayApp.Features.Holidays.Shared;
using HolidayApp.Shared.Abstract;
using HolidayApp.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Features.Holidays.GetLastCelebratedHolidays;

public sealed record GetLastCelebratedHolidaysRequest([FromRoute] string CountryCode);

public sealed record GetLastCelebratedHolidaysResponse(
    int Id,
    DateOnly Date,
    string Name
);

public class GetLastCelebratedHolidays : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet(HolidaysApiEndpoints.LastCelebrated, Handle)
            .AddEndpointFilter<ValidationFilter<GetLastCelebratedHolidaysRequest>>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetLastCelebratedHolidaysRequest request,
        HolidaysDbContext dbContext,
        ILogger<GetLastCelebratedHolidays> logger,
        CancellationToken cancellationToken
    )
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var holidays = await dbContext.Holiday
            .Where(h => h.CountryCode == request.CountryCode
                        && h.Date < today)
            .OrderByDescending(h => h.Date)
            .Take(3)
            .Select(h => MapResponse(h))
            .ToListAsync(cancellationToken);

        return Results.Ok(holidays);
    }

    private static GetLastCelebratedHolidaysResponse MapResponse(Holiday holiday)
    {
        return new GetLastCelebratedHolidaysResponse(holiday.Id, holiday.Date, holiday.Name);
    }
}