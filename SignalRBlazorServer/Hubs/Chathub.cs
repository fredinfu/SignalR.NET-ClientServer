using Microsoft.AspNetCore.SignalR;
namespace SignalRBlazorServer.Hubs;

public class Chathub : Hub
{
    //public Task SendMessage(string message)
    //{
    //    var connectionId = Context.ConnectionId;
    //    string userDetail = $" Client Connection Id: {connectionId}";
    //    Clients.Client(connectionId).SendAsync("ForceLogout");
    //    return Clients.All.SendAsync("ReceiveMessage",  message + " " + userDetail);
    //}
    public Task SendMessage(string user, string message)
    {
        var connectionId = Context.ConnectionId;
        string userDetail = $" Client Connection Id: {connectionId}";
        //Clients.Client(connectionId).SendAsync("ForceLogout");
        return Clients.All.SendAsync("ReceiveMessage", connectionId, message);
    }

    public Task DisconnectUser(string connectionId, string message)
    {
        var task = Clients.Client(connectionId).SendAsync("ForceLogout", connectionId, message);
        return task;
    }
}
