namespace WebApi;

public class WeatherForecasts
{
    private static readonly string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static readonly WeatherForecast[] Forecasts = Enumerable
        .Range(1, 1000)
        .Select(static index => new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class WeatherForecastResult
{
    public int Count { get; set; }

    public WeatherForecast[] WeatherForecasts { get; set; } = Array.Empty<WeatherForecast>();
}
