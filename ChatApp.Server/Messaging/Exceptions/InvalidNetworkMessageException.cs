namespace ChatApp.Server.Messaging.Exceptions
{
    public class InvalidNetworkMessageException : Exception
    {
        public InvalidNetworkMessageException(): base("Network Message Is Invalid.") { }
    }
}
