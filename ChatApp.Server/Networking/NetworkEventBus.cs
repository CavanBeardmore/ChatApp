using System.Threading.Channels;
using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Networking
{
    public class NetworkEventBus : INetworkEventBus
    {
        private readonly ILogger<NetworkEventBus> _logger;
        private readonly Channel<object>? _channel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public event Action<NetworkAction>? NetworkActionReceived;
        public event Action<NetworkMessage>? NetworkMessageReceived;

        public NetworkEventBus(ILogger<NetworkEventBus> logger)
        {
            _channel = Channel.CreateUnbounded<object>();
            _cancellationTokenSource = new CancellationTokenSource();
            _logger = logger;
        }

        public void WriteToChannel(object networkEvent)
        {
            if (_channel == null) return;

            _logger.LogInformation("NetworkEventBus - WriteToChannel - writing network event to channel");
            _channel.Writer.TryWrite(networkEvent);
        }

        public async Task ReadFromChannelAsync()
        {
            try
            {
                if (_channel == null) return;
                _cancellationToken = _cancellationTokenSource.Token;

                _logger.LogInformation("NetworkEventBus - ReadFromChannelAsync - started reading network events from channel");
                await foreach (var networkEvent in _channel.Reader.ReadAllAsync(_cancellationToken))
                {
                    _logger.LogInformation("NetworkEventBus - ReadFromChannelAsync - read event from channel");
                    Handle(networkEvent);
                }
            }
            catch (OperationCanceledException) { }
        }

        public void StopReading()
        {
            _logger.LogInformation("NetworkEventBus - StopReading - Stopping reading from channel");
            _cancellationTokenSource.Cancel();
        }

        private void Handle(object networkEvent)
        {
            _logger.LogInformation("NetworkEventBus - Handle - Handling network event");
            if (networkEvent is NetworkAction action)
            {
                _logger.LogInformation("NetworkEventBus - Handle - invoking network action received event");
                NetworkActionReceived?.Invoke(action);
            }

            if (networkEvent is NetworkMessage message)
            {
                _logger.LogInformation("NetworkEventBus - Handle - invoking network message received event : {@Message}", message);
                NetworkMessageReceived?.Invoke(message);
            }
        }
    }
}
