using ChatApp.Server.Messaging;
using ChatApp.Server.Messaging.Exceptions;
using ChatApp.Server.Networking.Data;
using System.Text.Json;

namespace ChatApp.Server.Actioning
{
    public class ActionFormattingDecorator : IActionClient<NetworkAction>
    {
        private readonly ILogger<ActionFormattingDecorator> _logger;
        private readonly IActionClient<string> _actionClient;
        public ActionFormattingDecorator(ILogger<ActionFormattingDecorator> logger, IActionClient<string> client)
        {
            _logger = logger;
            _actionClient = client;
        }
        public void Received(NetworkAction action)
        {
            try
            {
                _logger.LogInformation("ActionFormattingDecorator - Received - received network action : {@Action}", action);

                var formattedAction = new {
                    Type = "ACTION",
                    Data = action
                };

                string jsonMessage = JsonSerializer.Serialize(formattedAction);

                _logger.LogInformation("ActionFormattingDecorator - Received - passing json serialised formatted action on to next action client");

                _actionClient.Received(jsonMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("ActionFormattingDecorator - Received - Caught an error when formatting network action : {Message}", ex.Message);
            }
        }
    }
}
