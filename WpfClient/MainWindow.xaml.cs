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

        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7036/chathub")
            .WithAutomaticReconnect()
            .Build();

        counterConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7036/counterhub")
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
            });

            return Task.CompletedTask;
        };

        connection.Closed += (sender) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = "Connection closed";
                messages.Items.Add(newMessage);
                openConnection.IsEnabled = true;
                closeConnection.IsEnabled = false;
                sendMessage.IsEnabled = false;
            });

            return Task.CompletedTask;
        };
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        if(connection is not null)
        {
            await connection.SendAsync("SendMessage", connection.ConnectionId, messageInput.Text);
        }
    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {
        connection.On<string, string>("ReceiveMessage", async (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message}";
                messages.Items.Add(newMessage);
            });

        });

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
            messages.Items.Add("Connection started");
            openConnection.IsEnabled = false;
            closeConnection.IsEnabled = true;
            sendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private void closeConnection_Click(object sender, RoutedEventArgs e)
    {
        disconnect();
    }

    private void disconnect()
    {
        connection.StopAsync();
        openConnection.IsEnabled = true;
        closeConnection.IsEnabled = false;
    }

    private async void openCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await counterConnection.StartAsync();
            openCounter.IsEnabled = false;
        } catch (Exception ex)
        {

        }

    }

    private async void incrementCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await counterConnection.InvokeAsync("AddToTotal", "WPF Client", 1);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async void loginConnection_Click(object sender, RoutedEventArgs e)
    {
        
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



    }
}