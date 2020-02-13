using System;
namespace baxochat.Shared.ChatClient.Events
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string username, string message)
        {
            UserName = username;
            Message = message;
        }

        public string UserName { get; set; }

        public string Message { get; set; }

    }
}
