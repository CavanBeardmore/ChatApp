using System.Threading.Channels;

namespace ChatApp.Server.SSE
{
    public interface ISSEHandler
    {
        ChannelReader<string> Reader();
        void AddEvent(string chatAppEvent);
    }
}
