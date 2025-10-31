namespace ChatApp.Server.Networking.Data
{
    public class NetworkAction
    {
        public ActionType Type { get; }
        public string Username { get; }
        public string Timestamp { get; }

        public NetworkAction(ActionType type, string username, string timestamp)
        {
            Type = type;
            Username = username;
            Timestamp = timestamp;
        }
        public static bool IsValid(NetworkAction? action)
        {
            if (action == null) return false;

            if (!Enum.IsDefined(typeof(ActionType), action.Type)) return false;

            if (string.IsNullOrEmpty(action.Username)) return false;

            if (string.IsNullOrEmpty (action.Timestamp)) return false;

            return true;
        }
    }
}
