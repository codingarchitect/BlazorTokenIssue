using Serilog;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(static (_, cfg) => cfg
    .MinimumLevel.Debug()
    .WriteTo.Console());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication()
    .AddJwtBearer(static options =>
    {
        options.Audience = "webapi";
        options.Authority = "https://localhost:5001";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/weatherforecast", static (int skip = 0, int take = 10) =>
{
    var forecast =  WeatherForecasts.Forecasts
        .Skip(skip)
        .Take(take)
        .ToArray();

    return new WeatherForecastResult
    {
        WeatherForecasts = forecast,
        Count = WeatherForecasts.Forecasts.Length,
    };
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization();

app.Run();
