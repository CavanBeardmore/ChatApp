using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Networking
{
    public interface INetworkEventBus
    {
        event Action<NetworkAction>? NetworkActionReceived;
        event Action<NetworkMessage>? NetworkMessageReceived;
        void WriteToChannel(object networkEvent);
        Task ReadFromChannelAsync();
        void StopReading();
    }
}
