using Microsoft.AspNetCore.SignalR;

namespace SignalrServer.Hubs
{
	public class MessageHub : Hub
	{
		public async Task Broadcast(string username, string page) => await Clients.All.SendAsync("Broadcast", username, page);

		public override Task OnConnectedAsync() => base.OnConnectedAsync();

		public override async Task OnDisconnectedAsync(Exception e) => await base.OnDisconnectedAsync(e);
	}
}
