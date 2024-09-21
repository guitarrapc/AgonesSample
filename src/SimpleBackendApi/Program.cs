using SimpleBackendApi.Infrastructures;

namespace SimpleBackendApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var option = new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = Path.GetDirectoryName(typeof(Program).Assembly.Location),
        };
        var builder = WebApplication.CreateBuilder(option);
        builder.Configuration.AddJsonFile($"appsettings.Api.json");
        builder.Configuration.AddJsonFile($"appsettings.Api.{builder.Environment.EnvironmentName}.json", optional: true);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure Server Listener
        builder.ConfigureEndpoint();

        var app = builder.Build();

        // let's allow allow swagger even not Development
        app.UseSwagger();
        app.UseSwaggerUI();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
               new WeatherForecast
               (
                   DateTime.Now.AddDays(index),
                   Random.Shared.Next(-20, 55),
                   summaries[Random.Shared.Next(summaries.Length)]
               ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");

        await app.RunAsync();
    }

    internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
