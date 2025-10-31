using ChatApp.Server.Networking.Data;
using ChatApp.Server.Networking.Exceptions;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChatApp.Server.Networking.TCP
{
    public class TcpHost : INetworkHost
    {
        private readonly char _messageDelimiter = '\0';

        private readonly IPAddress _ipAddress;
        private readonly int _port;

        private readonly ILogger<TcpHost> _logger;

        private readonly INetworkEventBus _bus;

        private readonly ConcurrentDictionary<Guid, TcpClient> _clients;
        private readonly ConcurrentBag<Task> _clientTasks;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private TcpListener? _listener;

        public TcpHost(ILogger<TcpHost> logger, IPAddress ipAddress, int port, INetworkEventBus messageBus)
        {
            _logger = logger;
            _bus = messageBus;
            _ipAddress = ipAddress;
            _port = port;
            _cancellationTokenSource = new CancellationTokenSource();
            _clients = new();
            _clientTasks = new();
        }

        public async Task StartHostingAsync()
        {
            _listener = new TcpListener(_ipAddress, _port);

            _listener.Start();
            _logger.LogInformation("TcpHost - StartHostingAsync - Server Started");
            _cancellationToken = _cancellationTokenSource.Token;

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _logger.LogInformation("TcpHost - StartHostingAsync - Client connected");
                    Task clientTask = Task.Run(() => HandleClientAsync(client, _cancellationToken));
                    _clientTasks.Add(clientTask);
                    _ = clientTask;
                }
                catch (ObjectDisposedException)
                {
                    _logger.LogInformation("TcpHost - StartHostingAsync - disposed exception");
                    break;
                }
                catch (SocketException)
                {
                    _logger.LogInformation("TcpHost - StartHostingAsync - socket exception");
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            Guid guid = Guid.NewGuid();
            try
            {
                _clients.AddOrUpdate(guid, client, (key, _) => client);
                while (!cancellationToken.IsCancellationRequested)
                {
                    string? message = await ReadAsync(client, cancellationToken);

                    if (message == null)
                    {
                        _logger.LogInformation("TcpHost - HandleClientAsync - Client disconnected");
                        break;
                    }

                    _logger.LogInformation("TcpHost - HandleClientAsync - Received data");
                    var actionMessage = JsonSerializer.Deserialize<NetworkAction>(message);

                    if (NetworkAction.IsValid(actionMessage))
                    {
                        _logger.LogInformation("TcpHost - HandleClientAsync - writing action to bus");
                        _bus.WriteToChannel(actionMessage!);
                    }

                    var networkMessage = JsonSerializer.Deserialize<NetworkMessage>(message) ?? throw new NetworkMessageDeserializeException();

                    if (NetworkMessage.IsValid(networkMessage))
                    {
                        _logger.LogInformation("TcpHost - HandleClientAsync - writing message to bus");
                        _bus.WriteToChannel(networkMessage!);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                DisposeClient(client);
                _clients.TryRemove(guid, out _);
            }
        }

        public async Task BroadcastAsync(string message)
        {
            List<Task> tasks = new();
            string messageWithDelimiter = message + _messageDelimiter;

            foreach (KeyValuePair<Guid, TcpClient> kvp in _clients)
            {
                tasks.Add(SafeWriteAsync(kvp.Key, kvp.Value, message));
            }
            await Task.WhenAll(tasks);
        }

        private async Task SafeWriteAsync(Guid guid, TcpClient client, string message)
        {
            try
            {
                await WriteAsync(client, message);
            }
            catch
            {
                DisposeClient(client);
                _clients.TryRemove(guid, out _);
            }
        }

        private async Task WriteAsync(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"WRITING MESSAGE TO CLIENT {message}");
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task<string?> ReadAsync(TcpClient client, CancellationToken cancellationToken)
        {
            NetworkStream stream = client.GetStream();
            StringBuilder stringBuilder = new();
            byte[] buffer = new byte[1024];
            int byteCount;

            while ((byteCount = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));
                if (stringBuilder.ToString().Contains(_messageDelimiter))
                {
                    string message = stringBuilder.ToString().TrimEnd(_messageDelimiter);
                    stringBuilder.Clear();
                    return message;
                }
            }

            return null;
        }

        public async Task StopHostingAsync()
        {
            _logger.LogInformation("TcpHost - StopHostingAsync - stopping hosting");
            _cancellationTokenSource.Cancel();

            if (_listener != null) _listener.Stop();

            await Task.WhenAll(_clientTasks.ToArray());
        }

        private void DisposeClient(TcpClient client)
        {
            _logger.LogInformation("TcpHost - StopHostingAsync - disposing of client");
            client.Close();
            client.Dispose();
        }
    }
}
