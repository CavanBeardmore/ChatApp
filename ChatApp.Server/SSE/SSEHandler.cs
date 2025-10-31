using ChatApp.Server.Messaging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ChatApp.Server.SSE
{
    public class SSEHandler : ISSEHandler
    {
        private readonly Channel<string> _eventChannel;
        private readonly ILogger<SSEHandler> _logger;

        public SSEHandler(ILogger<SSEHandler> logger)
        {
            _eventChannel = Channel.CreateUnbounded<string>();
            _logger = logger;
        }

        public ChannelReader<string> Reader()
        {
            return _eventChannel.Reader;
        }

        public void AddEvent(string chatAppEvent)
        {
            _logger.LogInformation("SSEHandler - AddEvent - adding event {Event}", chatAppEvent);

            _eventChannel.Writer.TryWrite(chatAppEvent);
        }
    }
}
