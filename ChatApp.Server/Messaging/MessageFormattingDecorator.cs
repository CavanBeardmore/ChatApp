using ChatApp.Server.Messaging.Exceptions;
using ChatApp.Server.Networking.Data;
using System.Text.Json;

namespace ChatApp.Server.Messaging
{
    public class MessageFormattingDecorator : IMessageClient<NetworkMessage>
    {
        private readonly ILogger<MessageFormattingDecorator> _logger;
        private readonly IMessageClient<string> _messageClient;
        public MessageFormattingDecorator(ILogger<MessageFormattingDecorator> logger, IMessageClient<string> client) 
        {
            _logger = logger;
            _messageClient = client;
        }
        public void Received(NetworkMessage message)
        {
            try
            {
                _logger.LogInformation("MessageFormattingDecorator - Received - received network message : {@Message}", message);
                VerifyNetworkMessageIsValid(message);

                var formattedMessage = new
                {
                    Type = "MESSAGE",
                    Data = new NetworkMessage(
                    message.Text,
                    message.Sender.ToUpper(),
                    message.Timestamp
                    )
                };

                _logger.LogInformation("MessageFormattingDecorator - Received - formatted network message : {@Formatted}", formattedMessage);

                string jsonMessage = JsonSerializer.Serialize(formattedMessage);

                _logger.LogInformation("MessageFormattingDecorator - Received - passing json serialised formatted message on to next message client");

                _messageClient.Received(jsonMessage);
            }
            catch (InvalidNetworkMessageException) 
            {
                _logger.LogError("MessageFormattingDecorator - Received - Network message is invalid");
            }
            catch (Exception ex)
            {
                _logger.LogError("MessageFormattingDecorator - Received - Caught an error when formatting network message : {Message}", ex.Message);
            }
        }

        public void Send(NetworkMessage message)
        {
            _logger.LogInformation("MessageFormattingDecorator - Send - sending network message : {@Message}", message);
            VerifyNetworkMessageIsValid(message);

            string jsonMessage = JsonSerializer.Serialize(message);

            _logger.LogInformation("MessageFormattingDecorator - Send - sending json serialised network message");

            _messageClient.Send(jsonMessage);
        }

        private void VerifyNetworkMessageIsValid(NetworkMessage message)
        {
            _logger.LogInformation("MessageFormattingDecorator - VerifyNetworkMessageIsValid - Verifying network message is valid");
            if (message == null || message.Text == null || message.Sender == null || message.Text == null)
            {
                throw new InvalidNetworkMessageException();
            }
        }
    }
}
