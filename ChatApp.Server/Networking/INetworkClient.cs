namespace ChatApp.Server.Networking
{
    public interface INetworkClient
    {
        void StartClient(string username);
        Task WriteAsync(string message);
        Task StopClientAsync();
    }
}
