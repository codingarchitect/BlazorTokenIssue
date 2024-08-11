using BlazorApp;
using BlazorApp.Circuits;
using BlazorApp.Components;
using BlazorApp.Data.Chat;
using BlazorApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(static (ctx, sp, cfg) => cfg
    .ReadFrom.Services(sp)
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, static options =>
    {
        options.Authority = "https://localhost:5001";
        options.ClientId = "blazor-app";
        options.ClientSecret = "244c6179-9650-4f99-be2b-e2265cccef77";
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("webapi");
        options.ResponseType = "code";
        options.UsePkce = true;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SignedOutRedirectUri = "https://localhost:7055";
    });

builder.Services.AddTransient<AuthenticationStateHandler>();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient("weather")
    .AddHttpMessageHandler<AuthenticationStateHandler>();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<WeatherForecastService>();

builder.Services.AddCircuitServicesAccessor();
builder.Services.AddMudServices();
builder.Services.AddSignalRService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .RequireAuthorization(new AuthorizeAttribute
    {
        AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme
    })
    .AddInteractiveServerRenderMode();
app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});
app.MapHub<ChatHubControl>(ChatHubControl.__HubUrl);

app.Run();
