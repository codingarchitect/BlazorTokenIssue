using SignalrServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/", () => "Hello World, Signalr Server is running on http://localhost:7085!");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
	endpoints.MapHub<MessageHub>("/chat");
});

app.Run();
