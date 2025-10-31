using ChatApp.Server.Networking;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Server.Networking.Data;
using ChatApp.Server.Host.Models;

namespace ChatApp.Server.Host
{
    [ApiController]
    [Route("[controller]")]
    public class HostController : ControllerBase
    {
        private readonly ILogger<HostController> _logger;
        private readonly IHostFacade _host;

        public HostController(ILogger<HostController> logger, IHostFacade host)
        {
            _logger = logger;
            _host = host;
        }

        [HttpPost("start-host", Name = "StartHost")]
        public IActionResult Start()
        {
            if (_host.IsHosting) return BadRequest("Host Is Already Running.");

            _host.StartHost();
            return Ok();
        }

        [HttpPost("stop-host", Name = "StopHost")]
        public async Task<IActionResult> Stop()
        {
            await _host.StopHostAsync();
            return Ok();
        }

        [HttpPost("send-message-host", Name = "SendMessageHost")]
        public IActionResult SendMessage([FromBody] HostSendMessageModel model)
        {
            NetworkMessage message = new NetworkMessage(model.Message, model.Username, DateTime.UtcNow.ToString());
            _host.SendMessage(message);
            return Ok(message);
        }
    }
}
