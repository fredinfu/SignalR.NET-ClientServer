using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace WpfClient;

public partial class MainWindow : Window
{
    HubConnection connection;
    public MainWindow()
    {
        InitializeComponent();

        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7036/chathub")
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
            await connection.SendAsync("SendMessage", "WPF Client", messageInput.Text);
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
        connection.StopAsync();
        openConnection.IsEnabled = true;
        closeConnection.IsEnabled = false;
    }
}