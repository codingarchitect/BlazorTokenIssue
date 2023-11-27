# Blazor Token Issue

This is a small example solution to demonstrate an issue safely getting the
user's `access_token` from inside an interactive callback to the server.

To run the project using _Tye_:

``` shell
dotnet tool restore &&
    dotnet tye run
```

## Problem

When making requests in InteractiveServer mode, the user's access token is required.

The following documentation was used to try and implement this behaviour:

* [Server-side ASP.NET Core Blazor additional security scenarios](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-8.0)
  * [Pass tokens to a server-side Blazor app](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-8.0#pass-tokens-to-a-server-side-blazor-app)
  * [Set the authentication scheme](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-8.0#set-the-authentication-scheme)
  * [Access AuthenticationStateProvider in outgoing request middleware](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-8.0#access-authenticationstateprovider-in-outgoing-request-middleware)
* [ASP.NET Core Blazor dependency injection](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-8.0)
  * [Access server-side Blazor services from a different DI scope](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-8.0#access-server-side-blazor-services-from-a-different-di-scope)

## Test Solution

This solution is comprised of 3 projects

1. `WebApi` - This is a small Web API project that returns a set of weather
   forecasts. The service is configured to require an access token with the
   `webapi` audience.

2. `IdentityService` - This is an in memory implementation of Identity Server 4,
   the client and resources for `WebAPI` is added by default, along with two
   test users:
   * Alice - Username: `alice`, password: `alice`
   * Bob - Username: `bob`, password: `bob`

3. `BlazorApp` - This is a .NET 8 Blazor App. There app is configured to require
   authentication with OpenIDConnect, via `IdentityService`. The tokens are
   stored into the `TokenProvider` as showin in the documentation.

## Issues

1. The token provider correctly contains the access token for the user when
   called via a server side rendered page, however when called via the websocket
   the token provider is not configured.

2. The `CreateInboundActivityHandler` method of the `CircuitHandler` is never
   called. As a result, the `CircuitServicesAccessor` pattern described in the
   documentation does not work as the services are never set on the accessor.
   The circuit handler is loaded, as the calls to `OnConnectionUpAsync`,
   `OnCircuitOpenedAsync`, `OnCircuitClosedAsync`, and `OnConnectionDownAsync`
   all appear in the log as expected, however the interactive calls to the
   server never seem to trigger the inbound activity handler.
