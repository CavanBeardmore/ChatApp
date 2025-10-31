using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace ChatApp.Server.SSE
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : Controller
    {
        private readonly ISSEHandler _sseHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(ISSEHandler sseHandler, ILogger<EventController> logger)
        {
            _sseHandler = sseHandler;
            _logger = logger;
        }

        [HttpGet()]
        public async Task Stream(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("EventController - settings content type header");
                Response.Headers["Content-Type"] = "text/event-stream";

                await Response.Body.FlushAsync();
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("EventController - waiting to read from event service channel");
                    ChannelReader<string> channelReader = _sseHandler.Reader();

                    var hasData = await channelReader.WaitToReadAsync(cancellationToken);
                    if (!hasData)
                    {
                        _logger.LogInformation("EventController  - channel closed");
                        break;
                    }

                    while (channelReader.TryRead(out var chatAppEvent))
                    {
                        _logger.LogInformation("EventController - processing event");
                        await Response.WriteAsync($"data: {chatAppEvent}\n\n");
                        await Response.Body.FlushAsync();
                    }
                }
                _logger.LogInformation("EventController - cancellation requested");
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("EventController - Stream - request cancelled");
            }
        }
    }
}
