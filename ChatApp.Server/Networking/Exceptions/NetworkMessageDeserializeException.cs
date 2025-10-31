namespace ChatApp.Server.Networking.Exceptions
{
    public class NetworkMessageDeserializeException : Exception
    {
        public NetworkMessageDeserializeException() : base("Failed to deserialize network message.") { }
    }
}
