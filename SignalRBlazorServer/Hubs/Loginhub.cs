using Microsoft.AspNetCore.SignalR;
namespace SignalRBlazorServer.Hubs;

public class Loginhub : Hub
{
    public Task SendMessage(string user, string message)
    {
        var connectionId = Context.ConnectionId;
        string userDetail = $" Client Connection Id: {connectionId}";
        //Clients.Client(connectionId).SendAsync("ForceLogout");
        return Clients.All.SendAsync("ReceiveMessage", connectionId, message);
    }

    public Task LoginMessage(string connectionId, string username)
    {
        var loginMessage = $"User: {username} has joined";
        return Clients.All.SendAsync("ReceiveMessage", connectionId, loginMessage);
    }

    public Task DisconnectUser(string connectionId, string message)
    {
        var task = Clients.Client(connectionId).SendAsync("ForceLogout", connectionId, message);
        return task;
    }

}
