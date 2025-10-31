using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Client
{
    public interface IClientFacade
    {
        void StartClient(string username);
        Task StopClientAsync();
        void SendMessage(NetworkMessage message);
    }
}
