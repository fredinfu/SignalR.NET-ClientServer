using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace WpfClient;

public partial class MainWindow : Window
{
    HubConnection connection;
    HubConnection counterConnection;
    public MainWindow()
    {
        InitializeComponent();
        usernameInput.ForceCursor = true;
        connection = new HubConnectionBuilder()
            //.WithUrl("https://localhost:5012/loginhub") //GenesysOneApi
            .WithUrl("https://localhost:7036/loginhub") //BlazorSignalServer
            .WithAutomaticReconnect()
            .Build();

        connection.Reconnecting += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage);
                
            });

            return Task.CompletedTask;
        };

        connection.Reconnected += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Reconnected to the server";
                messages.Items.Clear();
                messages.Items.Add(newMessage);
                connectionId.Text = connection.ConnectionId;
            });

            return Task.CompletedTask;
        };

        connection.Closed += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Connection closed";
                messages.Items.Add(newMessage);
                closeConnection.IsEnabled = false;
                sendMessage.IsEnabled = false;
                connectionId.Text = "";
            });

            return Task.CompletedTask;
        };
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        if(connection is not null)
        {
            await connection.SendAsync("SendMessage", connection.ConnectionId, messageInput.Text, usernameInput.Text);
        }
    }

    private async void loginConnection_Click(object sender, RoutedEventArgs e)
    {
        //Initiate handler to start listening new messages sents on "ReceiveMessage" MethodName Channel
        connection.On<string, string, string>("ReceiveMessage", async (connectionId, message, user) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message} (ConnectionId: {connectionId})";
                messages.Items.Add(newMessage);
            });

        });

        //Initiante event handler listening when server condition indicates this user to disconnect
        connection.On<string, string>("ForceLogout", async (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"You have been disconnected {user}: {message}";
                messages.Items.Add(newMessage);
                disconnect();
            });

        });

        try
        {
            await connection.StartAsync();
            messages.Items.Add("Login started");
            loginConnection.IsEnabled = false;
            closeConnection.IsEnabled = true;
            sendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }

        if (connection is not null)
        {
            //Sends info to server, server should save in db ConnectionId of this session where Username matches in a SessionManagement Table
            await connection.SendAsync("LoginMessage", connection.ConnectionId, usernameInput.Text);
            connectionId.Text = connection.ConnectionId;
            messageInput.ForceCursor = true;
        }


    }



    private async void closeConnection_Click(object sender, RoutedEventArgs e)
    {
        if (connection is not null)
        {
            await connection.SendAsync("SendMessage", connection.ConnectionId, $"Connection :{connection.ConnectionId} has log out");
        }
        disconnect();
    }

    private void disconnect()
    {
        connection.StopAsync();
        loginConnection.IsEnabled = true;
        closeConnection.IsEnabled = false;
        
    }

    private void usernameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
  {
        loginConnection.IsEnabled = string.IsNullOrEmpty(usernameInput.Text) == false;
        
    }

    private void clearMessages_Click(object sender, RoutedEventArgs e)
    {
        messages.Items.Clear();
    }
}