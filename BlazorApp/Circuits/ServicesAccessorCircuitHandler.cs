using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorApp.Circuits;

public class ServicesAccessorCircuitHandler : CircuitHandler
{
    private readonly IServiceProvider _services;
    private CircuitServicesAccessor _circuitServicesAccessor;
    private readonly ILogger<ServicesAccessorCircuitHandler> _logger;

    public ServicesAccessorCircuitHandler(
        IServiceProvider services,
        CircuitServicesAccessor circuitServicesAccessor,
        ILogger<ServicesAccessorCircuitHandler> logger)
    {
        _services = services;
        _circuitServicesAccessor = circuitServicesAccessor;
        _logger = logger;
    }

    public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
        Func<CircuitInboundActivityContext, Task> next)
    {
        _logger.LogInformation("CreateInboundActivityHandler called");

        return async context =>
        {
            _logger.LogInformation("CreateInboundActivityHandler run for circuit {Circuit}", context.Circuit.Id);
            _circuitServicesAccessor.Services = _services;
            await next(context);
            _circuitServicesAccessor.Services = null;
        };
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnConnectionUpAsync for circuit {Circuit}", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnCircuitOpenedAsync for circuit {Circuit}", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnCircuitClosedAsync for circuit {Circuit}", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnConnectionDownAsync for circuit {Circuit}", circuit.Id);
        return Task.CompletedTask;
    }
}
