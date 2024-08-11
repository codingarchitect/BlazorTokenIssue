using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.Data.Chat;

public class ChatHubControl : Hub
{
    public const string __HubUrl = "/chat";

    public async Task Broadcast(string username, string page) => await Clients.All.SendAsync("Broadcast", username, page);

    public override Task OnConnectedAsync() => base.OnConnectedAsync();

    public override async Task OnDisconnectedAsync(Exception e) => await base.OnDisconnectedAsync(e);
}
