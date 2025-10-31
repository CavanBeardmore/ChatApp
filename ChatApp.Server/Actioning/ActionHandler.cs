using ChatApp.Server.Networking.Data;

namespace ChatApp.Server.Actioning
{
    public class ActionHandler : IActionClient<string>
    {
        private readonly ILogger<ActionHandler> _logger;

        public event Action<string>? ReceivedAction;

        public ActionHandler(ILogger<ActionHandler> logger)
        {
            _logger = logger;
        }

        public void Received(string action)
        {
            _logger.LogInformation("ActionHandler - Received - action : {@Action}", action);
            ReceivedAction?.Invoke(action);
        }
    }
}
