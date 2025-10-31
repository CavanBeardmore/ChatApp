using ChatApp.Server.Messaging;
using ChatApp.Server.Networking;
using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Host
{
    public class HostFacade : IHostFacade
    {
        private readonly ILogger<HostFacade> _logger;
        private readonly INetworkHost _networkHost;
        private readonly INetworkEventBus _networkEventBus;
        private readonly IMessageClient<NetworkMessage> _messageClient;
        private readonly IMessagingQueue _messagingQueue;

        public bool IsHosting { get; private set; }

        public HostFacade(ILogger<HostFacade> logger, INetworkHost host, INetworkEventBus networkEventBus, IMessageClient<NetworkMessage> messageClient, IMessagingQueue messagingQueue) 
        {
            _logger = logger;
            _networkHost = host;
            _networkEventBus = networkEventBus;
            _messageClient = messageClient;
            _messagingQueue = messagingQueue;
        }

        public void SendMessage(NetworkMessage message)
        {
            _logger.LogInformation("HostFacade - SendMessage - message: {Message}", message);
            _messageClient.Send(message);
        }

        public void StartHost()
        {
            try
            {
                _logger.LogInformation("HostFacade - StartHost - starting host processes");

                IsHosting = true;

                Task.Run(async () => {
                    try
                    {
                        _logger.LogInformation("HostFacade - StartHost - starting network host process");
                        await _networkHost.StartHostingAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("HostFacade - StartHost - Caught error running network host process : {Message}", ex.Message);
                    }
                });

                Task.Run(async () => {
                    try
                    {
                        _logger.LogInformation("HostFacade - StartHost - starting network event bus process");
                        await _networkEventBus.ReadFromChannelAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("HostFacade - StartHost - Caught error running network event bus process : {Message}", ex.Message);
                    }
                });

                Task.Run(async () => {
                    try
                    {
                        _logger.LogInformation("HostFacade - StartHost - starting message queue process");
                        await _messagingQueue.ReadFromChannelAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("HostFacade - StartHost - Caught error running message queue process : {Message}", ex.Message);
                    }
                });
            }
            catch
            {
                IsHosting = false;
            }
        }

        public async Task StopHostAsync()
        {
            try
            {
                _logger.LogInformation("HostFacade - StopHostAsync - ending network host process");
                await _networkHost.StopHostingAsync();
            } 
            catch (Exception ex)
            {
                _logger.LogInformation("HostFacade - StopHostAsync - Caught error ending network host process : {Message}", ex.Message);
            }

            try
            {
                _logger.LogInformation("HostFacade - StopHostAsync - ending network event bus process");
                _networkEventBus.StopReading();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("HostFacade - StopHostAsync - Caught error ending network event bus process : {Message}", ex.Message);
            }

            try
            {
                _logger.LogInformation("HostFacade - StopHostAsync - ending messaging queue process");
                _messagingQueue.StopReading();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("HostFacade - StopHostAsync - Caught error ending messaging queue process : {Message}", ex.Message);
            }

            IsHosting = false;
        }
    }
}
