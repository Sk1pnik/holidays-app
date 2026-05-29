using HolidayApp.Database;
using HolidayApp.Shared.Abstract;
using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Shared.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolidaysDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<HolidaysDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("HolidaysDatabaseConnection"),
                sql => sql.EnableRetryOnFailure(maxRetryCount: 3)));

        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = typeof(IEndpoint).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IEndpoint)));

        foreach (var type in endpointTypes)
            services.AddSingleton(typeof(IEndpoint), type); 

        return services;
    }
}