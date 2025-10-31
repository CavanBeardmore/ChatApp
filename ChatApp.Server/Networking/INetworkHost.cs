namespace ChatApp.Server.Networking
{
    public interface INetworkHost
    {
        Task StartHostingAsync();
        Task BroadcastAsync(string message);
        Task StopHostingAsync();
    }
}
