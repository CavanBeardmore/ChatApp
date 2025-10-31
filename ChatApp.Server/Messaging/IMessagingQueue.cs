using System.Threading.Channels;

namespace ChatApp.Server.Messaging
{
    public interface IMessagingQueue
    {
        Task ReadFromChannelAsync();
        void StopReading();
    }
}
