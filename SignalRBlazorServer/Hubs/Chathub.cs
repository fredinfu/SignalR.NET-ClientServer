using Microsoft.AspNetCore.SignalR;
namespace SignalRBlazorServer.Hubs;

public class Chathub : Hub
{
    public Task SendMessage(string user, string message)
    {
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
