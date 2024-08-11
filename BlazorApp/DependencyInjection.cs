using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR;
using BlazorApp.Data.Chat;
using Microsoft.Extensions.Configuration;

namespace BlazorApp;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRService(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddSingleton<ChatBroadcastModel>();
        var useLocalSignalR = configuration.GetValue<bool>("UseLocalSignalR");
        var signalRConnectionString = configuration.GetConnectionString("SignalR");
        if (useLocalSignalR)
        {
            services.AddSignalR(s =>
            {
                s.EnableDetailedErrors = true;
            });
        }
        else
        {
            services.AddSignalR(s =>
            {
                s.EnableDetailedErrors = true;
            }).AddAzureSignalR(options =>
            {
                options.ServerStickyMode = ServerStickyMode.Required;
                options.ConnectionString = signalRConnectionString;
            });
        }
        
		return services;
    }
    
}
