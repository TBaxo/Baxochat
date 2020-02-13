using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using baxochat.Shared.ChatClient;
using Microsoft.AspNetCore.SignalR;

namespace baxochat.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, string> userDictionary = new Dictionary<string, string>();

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync(MessageTypes.RECEIVED, userName, message);
        }

        public async Task Connect(string userName)
        {
            string currentId = Context.ConnectionId;
            if (!userDictionary.ContainsKey(currentId))
            {
                userDictionary.Add(currentId, userName);
                await Clients.All.SendAsync(MessageTypes.RECEIVED, "Server", $"{userName} has joined the chat");
            }

        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string currentId = Context.ConnectionId;
            string userName = userDictionary[currentId];

            userDictionary.Remove(currentId);

            await Clients.AllExcept(currentId).SendAsync(MessageTypes.RECEIVED, "Server", $"{userName} HashCode left the chat");

            await base.OnDisconnectedAsync(exception);
        }
    }
}