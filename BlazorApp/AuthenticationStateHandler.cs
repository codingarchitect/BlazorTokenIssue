using System.Net.Http.Headers;
using BlazorApp.Circuits;
using BlazorApp.Services;

namespace BlazorApp;

public class AuthenticationStateHandler : DelegatingHandler
{
    private readonly CircuitServicesAccessor _circuitServicesAccessor;
    private readonly ILogger<AuthenticationStateHandler> _logger;

    public AuthenticationStateHandler(
        CircuitServicesAccessor circuitServicesAccessor,
        ILogger<AuthenticationStateHandler> logger)
    {
        _circuitServicesAccessor = circuitServicesAccessor;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is not { Scheme: "Bearer", Parameter: not null })
        {
            var tokenProvider = _circuitServicesAccessor.Services?.GetRequiredService<TokenProvider>();
            if (tokenProvider is { AccessToken: { } accessToken })
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _logger.LogInformation("Access token header added to request");
            }
            else
            {
                _logger.LogError(
                    "Failed to load the token provider with access token, has token provider: {TokenProvider}, has access token: {AccessToken}",
                    tokenProvider != null, tokenProvider?.AccessToken != null);
            }
        }
        else
        {
            _logger.LogDebug("Request already had an bearer token");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
