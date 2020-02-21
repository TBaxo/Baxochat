namespace baxochat.Client.Shared.Models
{
    public class Message
    {
        public Message(string userName, string content)
        {
            UserName = userName;
            Content = content;
        }

        public string UserName;

        public string Content;

        public bool IsMine;
    }
}