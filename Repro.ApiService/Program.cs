using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = CreateSerilogLogger(builder.Configuration);

builder.Logging.AddSerilog();
builder.Host.UseSerilog();
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});
app.MapDefaultEndpoints();

app.Run();
static ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", "Repro App")
        .Enrich.FromLogContext()
        .WriteTo.Console()
        //comment this line out, when you want to see it doesn't write to "Structured Logs", however tracing just works fine.
        .WriteTo.OpenTelemetry(options =>
        {
            options.Endpoint = "http://localhost:16003";
            options.ResourceAttributes.Add("service.name", "apiservice");
        })
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


