namespace ChatApp.Server.Networking.Data
{
    public class NetworkMessage
    {
        public string Text { get; }
        public string Sender { get; }
        public string Timestamp { get; }

        public NetworkMessage(string text, string sender, string timestamp) 
        {
            Text = text;
            Sender = sender;
            Timestamp = timestamp;
        }

        public static bool IsValid(NetworkMessage? message)
        {
            if (message == null) return false;

            if (string.IsNullOrEmpty(message.Text)) return false;

            if (string.IsNullOrEmpty(message.Sender)) return false;

            if (string.IsNullOrEmpty(message.Timestamp)) return false;

            return true;
        }
    }
}
