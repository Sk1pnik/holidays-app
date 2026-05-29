using FluentValidation;
using HolidayApp.Features.Holidays.SyncHolidays;

namespace HolidayApp.Features.Holidays.Shared.Infrastructre;

public static class HolidayExtensions
{
    public static IServiceCollection AddHolidayServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHolidayHttpClient()
            .AddHolidayValidation();

        return services;
    }

    private static IServiceCollection AddHolidayHttpClient(this IServiceCollection services)
    {
        services
            .AddHttpClient<IHolidaysClient, HolidayClient>(client =>
            {
                client.BaseAddress = new Uri("https://date.nager.at/api/v3/PublicHolidays/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddStandardResilienceHandler();

        return services;
    }

    private static IServiceCollection AddHolidayValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<HolidayAssemblyMarker>();
        return services;
    }
}