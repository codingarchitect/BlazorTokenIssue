using BlazorApp.Models;

namespace BlazorApp.Services;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly TokenProvider _tokenProvider;
    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(
        IHttpClientFactory clientFactory,
        TokenProvider tokenProvider,
        ILogger<WeatherForecastService> logger)
    {
        _httpClient = clientFactory.CreateClient("weather");
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<WeatherForecastResult> GetForecastAsync(int skip = 0, int take = 10)
    {
        var token = _tokenProvider.AccessToken;

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token is missing from request");
        }

        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://localhost:7262/WeatherForecast");
        request.Headers.Add("Authorization", $"Bearer {token}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WeatherForecastResult>() ??
               new WeatherForecastResult();
    }
}
