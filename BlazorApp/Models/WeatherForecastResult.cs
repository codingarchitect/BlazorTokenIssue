namespace BlazorApp.Models;

public class WeatherForecastResult
{
    public int Count { get; set; }

    public WeatherForecast[] WeatherForecasts { get; set; } = Array.Empty<WeatherForecast>();
}
