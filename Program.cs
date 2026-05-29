using HolidayApp.Features.Holidays.Shared.Infrastructre;
using HolidayApp.Shared.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddHolidaysDatabase(builder.Configuration)
    .AddEndpoints()
    .AddHolidayServices(builder.Configuration);

var app = builder.Build();

await app.EnsureDatabaseCreatedAsync();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapEndpoints();

await app.RunAsync();