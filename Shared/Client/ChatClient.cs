using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using baxochat.Shared.ChatClient.Events;

namespace baxochat.Shared.ChatClient
{
    public class Client : IAsyncDisposable
    {
        //DI
        private readonly NavigationManager _navigationManager;

        //properties
        public static string HUB_URL = "/chatHub";
        private readonly string _userName;
        public bool _started = false;
        public HubConnection _hubConnection;


        public Client(string userName, NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _userName = userName;
        }

        public async Task StartAsync()
        {
            if (!_started)
            {
                Uri test = _navigationManager.ToAbsoluteUri(HUB_URL);
                // create the connection using the .NET SignalR client
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(test)
                    .Build();
                Console.WriteLine("ChatClient: calling Start()");

                // add handler for receiving messages
                _hubConnection.On<string, string>(MessageTypes.RECEIVED, (user, message) =>
                {
                    HandleReceiveMessage(user, message);
                });

                // start the connection
                await _hubConnection.StartAsync();

                Console.WriteLine("ChatClient: Start returned");
                _started = true;

                // register user on hub to let other clients know they've joined
                await _hubConnection.SendAsync(MessageTypes.CONNECT, _userName);
            }
        }

        public async Task SendMessage(string message)
        {
            await _hubConnection.SendAsync(MessageTypes.SEND, _userName, message);
        }

        private void HandleReceiveMessage(string username, string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(username, message));
        }

        public event MessageReceivedEventHandler MessageReceived;
        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

        public async Task StopAsync()
        {
            if (_started)
            {
                await _hubConnection.StopAsync();

                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _started = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("ChatClient: Disposing");
            await StopAsync();
        }

    }

}

