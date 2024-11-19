using DemoApi.Extensions;
using DemoApi.Services.Delay;
using DemoApi.Services.Random;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseSerilog((_, services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services
    .AddScoped<IRandomService, RandomService>()
    .AddScoped<IDelayService, DelayService>();

builder.ConfigureOpenTelemetry();

var app = builder.Build();
app.UseCors("AllowAll");
app.UseSerilogRequestLogging();
app.MapPrometheusScrapingEndpoint();

app.MapGet("/ok", async (IDelayService delayService, IRandomService randomService) =>
{
    await delayService.Delay(randomService.Next(5, 10));
    return Results.Ok();
});

app.MapGet("/unauthorized",
    async (IDelayService delayService, IRandomService randomService, ILogger<Program> logger) =>
    {
        await delayService.Delay(randomService.Next(10, 20));
        if (randomService.NextDouble() < 0.4)
        {
            logger.LogError("Error: Unauthorized access attempted.");
            return Results.Unauthorized();
        }

        return Results.Ok();
    });

app.MapGet("/not-found",
    async (IDelayService delayService, IRandomService randomService, ILogger<Program> logger) =>
    {
        await delayService.Delay(randomService.Next(20, 30));
        if (randomService.NextDouble() < 0.4)
        {
            logger.LogError("Error: Resource not found.");
            return Results.NotFound();
        }

        return Results.Ok();
    });

app.MapGet("/bad-request",
    async (IDelayService delayService, IRandomService randomService, ILogger<Program> logger) =>
    {
        await delayService.Delay(randomService.Next(30, 40));
        if (randomService.NextDouble() < 0.4)
        {
            logger.LogError("Error: Bad request made.");
            return Results.BadRequest();
        }

        return Results.Ok();
    });

app.MapGet("/internal-server-error",
    async (IDelayService delayService, IRandomService randomService, ILogger<Program> logger) =>
    {
        await delayService.Delay(randomService.Next(40, 50));
        if (randomService.NextDouble() < 0.4)
        {
            logger.LogError("Error: Internal server error occurred.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Results.Ok();
    });

app.Run();