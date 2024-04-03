using Microsoft.AspNetCore.SignalR;
namespace SignalRBlazorServer.Hubs;

public class Loginhub : Hub
{
    public Task SendMessage(string connectionId, string message, string username)
    {
        //var connectionId = Context.ConnectionId;
        string userDetail = $" Client Connection Id: {connectionId}";
        //Clients.Client(connectionId).SendAsync("ForceLogout");
        return Clients.All.SendAsync("ReceiveMessage", connectionId, message, username);
    }

    public Task LoginMessage(string connectionId, string username)
    {
        var loginMessage = $"User: {username} has joined";
        /*
         * var person = personAccountService.GetPerson(username)
         */
        //sessionComponentService.UpdateConnectionId(username, person.PersonAccountId)
        return Clients.All.SendAsync("ReceiveMessage", connectionId, loginMessage, username);
    }

    public Task DisconnectUser(string connectionId, string message)
    {
        var task = Clients.Client(connectionId).SendAsync("ForceLogout", connectionId, message);
        return task;
    }

}
