using ChatApp.Server.Host;
using ChatApp.Server.Networking;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Server.Networking.Data;
using ChatApp.Server.Client.Models;

namespace ChatApp.Server.Client
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientFacade _client;

        public ClientController(ILogger<ClientController> logger, IClientFacade client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpPost("start-client", Name = "StartClient")]
        public IActionResult Start([FromBody] ClientStartModel model)
        {
            _client.StartClient(model.Username);
            return Ok();
        }

        [HttpPost("stop-client", Name = "StopClient")]
        public async Task<IActionResult> Stop()
        {
            await _client.StopClientAsync();
            return Ok();
        }

        [HttpPost("send-message-client", Name = "SendMessageClient")]
        public IActionResult SendMessage([FromBody] ClientSendMessageModel model)
        {
            NetworkMessage message = new NetworkMessage(model.Message, model.Username, DateTime.UtcNow.ToString());
            _client.SendMessage(message);
            return Ok(message);
        }
    }
}
