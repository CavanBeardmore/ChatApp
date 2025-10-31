using ChatApp.Server.Host;
using ChatApp.Server.Messaging;
using ChatApp.Server.Networking;
using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Client
{
    public class ClientFacade : IClientFacade
    {
        private readonly ILogger<ClientFacade> _logger;
        private readonly INetworkClient _networkClient;
        private readonly INetworkEventBus _networkEventBus;
        private readonly IMessageClient<NetworkMessage> _messageClient;
        private readonly IMessagingQueue _messagingQueue;

        public ClientFacade(ILogger<ClientFacade> logger, INetworkClient client, INetworkEventBus networkEventBus, IMessageClient<NetworkMessage> messageClient, IMessagingQueue messagingQueue)
        {
            _logger = logger;
            _networkClient = client;
            _networkEventBus = networkEventBus;
            _messageClient = messageClient;
            _messagingQueue = messagingQueue;
        }

        public void StartClient(string username)
        {
            _logger.LogInformation("ClientFacade - StartClient - starting client processes");

            Task.Run(() => {
                try
                {
                    _logger.LogInformation("ClientFacade - StartClient - starting network client process");
                    _networkClient.StartClient(username);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ClientFacade - StartClient - Caught error running network client process : {Message}", ex.Message);
                }
            });

            Task.Run(async () => {
                try
                {
                    _logger.LogInformation("ClientFacade - StartClient - starting network event bus process");
                    await _networkEventBus.ReadFromChannelAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("ClientFacade - StartClient - Caught error running network event bus process : {Message}", ex.Message);
                }
            });

            Task.Run(async () => {
                try
                {
                    _logger.LogInformation("ClientFacade - StartClient - starting message queue process");
                    await _messagingQueue.ReadFromChannelAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("ClientFacade - StartStartClientHost - Caught error running message queue process : {Message}", ex.Message);
                }
            });
        }

        public async Task StopClientAsync()
        {
            try
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - ending network client process");
                await _networkClient.StopClientAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - Caught error ending network client process : {Message}", ex.Message);
            }

            try
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - ending network event bus process");
                _networkEventBus.StopReading();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - Caught error ending network event bus process : {Message}", ex.Message);
            }

            try
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - ending messaging queue process");
                _messagingQueue.StopReading();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ClientFacade - StopClientAsync - Caught error ending messaging queue process : {Message}", ex.Message);
            }
        }

        public void SendMessage(NetworkMessage message)
        {
            _logger.LogInformation("ClientFacade - SendMessage - message: {Message}", message);
            _messageClient.Send(message);
        }
    }
}
