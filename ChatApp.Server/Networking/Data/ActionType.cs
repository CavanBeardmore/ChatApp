using System.ComponentModel;

namespace ChatApp.Server.Networking.Data
{
    public enum ActionType
    {
        [Description("JOIN")]
        JOIN,
        [Description("LEAVE")]
        LEAVE,
        [Description("JOINED")]
        JOINED
    }
}
