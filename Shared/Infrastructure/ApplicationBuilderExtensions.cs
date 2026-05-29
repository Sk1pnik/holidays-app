using HolidayApp.Database;
using HolidayApp.Shared.Abstract;
using HolidayApp.Shared.Middleware;

namespace HolidayApp.Shared.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static async Task EnsureDatabaseCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HolidaysDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(app);

        return app;
    }
    
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}