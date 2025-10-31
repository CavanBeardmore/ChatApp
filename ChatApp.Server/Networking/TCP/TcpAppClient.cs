using ChatApp.Server.Networking.Data;
using ChatApp.Server.Networking.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatApp.Server.Networking.TCP
{
    public class TcpAppClient : INetworkClient
    {
        private readonly char _messageDelimiter = '\0';
        private readonly INetworkEventBus _bus;

        private readonly string _address;
        private readonly int _port;

        private readonly ILogger<TcpAppClient> _logger;

        private TcpClient? _client;

        private string? _username;

        public TcpAppClient(ILogger<TcpAppClient> logger, int port, string address, INetworkEventBus bus)
        {
            _logger = logger;
            _port = port;
            _address = address;
            _bus = bus;
        }

        public void StartClient(string username)
        {
            _username = username;
            _client = new TcpClient();
            _logger.LogInformation("TcpAppClient - StartClient - Connecting to IP {Address} and Port {Port}", _address, _port);
            _client.Connect(_address, _port);
            _ = Task.Run(() => HandleConnectionToHostAsync());
        }

        private async Task HandleConnectionToHostAsync()
        {
            try
            {
                await WriteConnectedMessage();
                _bus.WriteToChannel(new NetworkAction(ActionType.JOINED, "SELF", DateTime.UtcNow.ToString()));

                while (_client != null && _client.Connected)
                {
                    string? message = await ReadAsync();

                    if (message == null)
                    {
                        _logger.LogInformation("TcpAppClient - HandleConnectionToHostAsync - host disconnected");

                        _bus.WriteToChannel(new NetworkAction(ActionType.LEAVE, "HOST", DateTime.UtcNow.ToString()));
                        break;
                    }

                    _logger.LogInformation("TcpAppClient - HandleConnectionToHostAsync - received data");

                    var actionMessage = JsonSerializer.Deserialize<NetworkAction>(message);

                    if (NetworkAction.IsValid(actionMessage))
                    {
                        _logger.LogInformation("TcpAppClient - HandleConnectionToHostAsync - writing action to bus");
                        _bus.WriteToChannel(actionMessage!);
                    }

                    var networkMessage = JsonSerializer.Deserialize<NetworkMessage>(message) ?? throw new NetworkMessageDeserializeException();

                    if (NetworkMessage.IsValid(networkMessage))
                    {
                        _logger.LogInformation("TcpAppClient - HandleConnectionToHostAsync - writing message to bus");
                        _bus.WriteToChannel(networkMessage!);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("TcpAppClient - HandleConnectionToHostAsync - exception caught while maintaing connection to host : {Message}", ex.Message);
            }
            finally
            {
                await StopClientAsync();
            }
        }

        private async Task WriteConnectedMessage()
        {
            await WriteAsync(JsonSerializer.Serialize(new
            {
                Type = ActionType.JOIN,
                Timestamp = DateTime.UtcNow.ToString(),
                Username = _username
            }));
        }

        public async Task WriteAsync(string message)
        {
            if (_client == null) return;

            _logger.LogInformation("TcpAppClient - WriteAsync - writing message");
            string messageWithDelimiter = message + _messageDelimiter;

            NetworkStream stream = _client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(messageWithDelimiter);

            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task<string?> ReadAsync()
        {
            if (_client == null) return null;

            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[1024];

            int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (byteCount == 0) return null;

            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            return message;
        }

        public async Task StopClientAsync()
        {
            if (_client == null) return;
 
            _logger.LogInformation("TcpAppClient - StopClientAsync - Disconnecting client from server");

            await WriteLeaveMessageAsync();

            _client.Close();
            _client.Dispose();
            _client = null;
        }

        private async Task WriteLeaveMessageAsync()
        {
            if (_client == null) return;

            await WriteAsync(JsonSerializer.Serialize(new
            {
                Type = ActionType.LEAVE,
                Timestamp = DateTime.UtcNow.ToString(),
                Username = _username
            }));

            //flush and delay is done to allow host time to read leave message before client disconnects
            await FlushAsync(_client);
            await Task.Delay(100);
        }

        private async Task FlushAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            await stream.FlushAsync();
        }
    }
}
