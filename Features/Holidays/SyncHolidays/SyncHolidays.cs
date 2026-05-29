using ErrorOr;
using HolidayApp.Database;
using HolidayApp.Features.Holidays.Shared;
using HolidayApp.Shared.Abstract;
using HolidayApp.Shared.Extensions;
using HolidayApp.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Features.Holidays.SyncHolidays;

public sealed record SyncHolidaysRequest(
    int Year,
    string CountryCode
);

public sealed record SyncHolidaysResponse(
    int Id,
    DateOnly Date,
    string Name,
    string LocalName,
    string CountryCode
);

public class SyncHolidays : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost(HolidaysApiEndpoints.Sync, Handle)
            .AddEndpointFilter<ValidationFilter<SyncHolidaysRequest>>();
    }

    private static async Task<IResult> Handle(
        [FromBody] SyncHolidaysRequest request,
        HolidaysDbContext dbContext,
        ILogger<SyncHolidays> logger,
        IHolidaysClient holidaysClient,
        CancellationToken cancellationToken)
    {
        var holidays = await holidaysClient.GetHolidaysAsync(
            request.Year, request.CountryCode, cancellationToken);

        if (holidays is null)
        {
            logger.LogWarning(
                "No public holidays found for {CountryCode} in {Year}.",
                request.CountryCode, request.Year);
            return Error.NotFound(
                    description:
                    $"No public holidays found for country code {request.CountryCode} in year {request.Year}")
                .ToProblem();
        }

        List<Holiday> savedHolidays = [];
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            savedHolidays = holidays
                .Where(h => h.Date.Year == request.Year)
                .DistinctBy(h => new { h.Date, h.CountryCode, h.Name })
                .Select(MapToEntity)
                .ToList();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await dbContext.Holiday
                    .Where(h => h.CountryCode == request.CountryCode
                                && h.Date.Year == request.Year)
                    .ExecuteDeleteAsync(cancellationToken);

                dbContext.Holiday.AddRange(savedHolidays);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "An error occurred while syncing holidays for {CountryCode} in {Year}.",
                    request.CountryCode, request.Year);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return Results.Ok(savedHolidays.Select(MapToResponse));
    }

    private static Holiday MapToEntity(HolidayClientResponse holiday)
    {
        return new Holiday
        {
            Date = holiday.Date,
            Name = holiday.Name,
            LocalName = holiday.LocalName,
            CountryCode = holiday.CountryCode
        };
    }


    private static SyncHolidaysResponse MapToResponse(Holiday holiday)
    {
        return new SyncHolidaysResponse(holiday.Id, holiday.Date, holiday.Name, holiday.LocalName, holiday.CountryCode);
    }
}