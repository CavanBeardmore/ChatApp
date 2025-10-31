using ChatApp.Server.Networking.Data;
using System.Threading.Channels;

namespace ChatApp.Server.Messaging
{
    public class MessageHandler : IMessageClient<string>, IMessagingQueue
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly Channel<string>? _channel;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public MessageHandler(ILogger<MessageHandler> logger)
        {
            _channel = Channel.CreateUnbounded<string>();
            _cancellationTokenSource = new CancellationTokenSource();
            _logger = logger;
        }

        public event Action<string>? ReceivedMessage;
        public event Action<string>? SendingMessage;

        public void Send(string message) 
        {
            _logger.LogInformation("MessageHandler - Send");
            if (_channel == null)
            {
                _logger.LogInformation("MessageHandler - Send - channel is null not sending message");
                return;
            }

            _logger.LogInformation("MessageHandler - Send - writing message to channel");
            _channel.Writer.TryWrite(message);
        }

        public void Received(string message)
        {
            _logger.LogInformation("MessageHandler - Received - invoking received message event");
            ReceivedMessage?.Invoke(message);
        }

        public async Task ReadFromChannelAsync()
        {
            try
            {
                _logger.LogInformation("MessageHandler - ReadFromChannelAsync");
                if (_channel == null)
                {
                    _logger.LogInformation("MessageHandler - ReadFromChannelAsync - channel is null cannot read messages");
                    return;
                }

                _cancellationToken = _cancellationTokenSource.Token;

                _logger.LogInformation("MessageHandler - ReadFromChannelAsync - starting to read messages from channel");
                await foreach (var message in _channel.Reader.ReadAllAsync(_cancellationToken))
                {
                    _logger.LogInformation("MessageHandler - ReadFromChannelAsync - read message from channel : {@Message}", message);
                    SendingMessage?.Invoke(message);
                }
            }
            catch (OperationCanceledException) { }
        }

        public void StopReading()
        {
            _logger.LogInformation("MessageHandler - StopReading - stopping reading from channel");
            _cancellationTokenSource.Cancel();
        }
    }
}
