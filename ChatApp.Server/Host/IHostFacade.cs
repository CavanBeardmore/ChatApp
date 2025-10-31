using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Host
{
    public interface IHostFacade
    {
        bool IsHosting { get; }
        void StartHost();
        Task StopHostAsync();
        void SendMessage(NetworkMessage message);
    }
}
