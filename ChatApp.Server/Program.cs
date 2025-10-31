using ChatApp.Server.Actioning;
using ChatApp.Server.Client;
using ChatApp.Server.Host;
using ChatApp.Server.Messaging;
using ChatApp.Server.Networking;
using ChatApp.Server.Networking.Data;
using ChatApp.Server.Networking.TCP;
using ChatApp.Server.SSE;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Serilog;
using System.Net;
using System.Net.Sockets;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

int port = 5000;

string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
        {
            return ip.ToString();
        }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:50472") // Frontend port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Optional: if you're sending cookies
    });
});

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddSingleton<MessageHandler>((sp) =>
{
    ILogger<MessageHandler> logger = sp.GetRequiredService<ILogger<MessageHandler>>();
    return new MessageHandler(logger);
});

builder.Services.AddSingleton<IMessagingQueue, MessageHandler>((sp) =>
{
    MessageHandler messageHandler = sp.GetRequiredService<MessageHandler>();
    return messageHandler;
});

builder.Services.AddSingleton<IMessageClient<NetworkMessage>, MessageFormattingDecorator>((sp) =>
{
    ILogger<MessageFormattingDecorator> logger = sp.GetRequiredService<ILogger<MessageFormattingDecorator>>();
    MessageHandler messageHandler = sp.GetRequiredService<MessageHandler>();
    return new MessageFormattingDecorator(logger, messageHandler);
});

builder.Services.AddSingleton<ActionHandler>((sp) =>
{
    ILogger<ActionHandler> logger = sp.GetRequiredService<ILogger<ActionHandler>>();

    return new ActionHandler(logger);
});

builder.Services.AddSingleton<IActionClient<NetworkAction>, ActionFormattingDecorator>((sp) =>
{
    ILogger<ActionFormattingDecorator> logger = sp.GetRequiredService<ILogger<ActionFormattingDecorator>>();
    IActionClient<string> client = sp.GetRequiredService<ActionHandler> ();

    return new ActionFormattingDecorator(logger, client);
});

builder.Services.AddSingleton<INetworkEventBus, NetworkEventBus>((sp) => {
    IMessageClient<NetworkMessage> messagingClient = sp.GetRequiredService<IMessageClient<NetworkMessage>>();
    IActionClient<NetworkAction> actioningClient = sp.GetRequiredService<IActionClient<NetworkAction>>();

    ILogger<NetworkEventBus> logger = sp.GetRequiredService<ILogger<NetworkEventBus>>();

    NetworkEventBus bus = new NetworkEventBus(logger);

    bus.NetworkMessageReceived += (NetworkMessage message) => messagingClient.Received(message);
    bus.NetworkActionReceived += (NetworkAction action) => actioningClient.Received(action);

    return bus;
});

builder.Services.AddSingleton<INetworkHost>((sp) =>
{
    ILogger<TcpHost> logger = sp.GetRequiredService<ILogger<TcpHost>>();
    INetworkEventBus networkEventBus = sp.GetRequiredService<INetworkEventBus>();
    MessageHandler messageHandler = sp.GetRequiredService<MessageHandler>();

    TcpHost host = new TcpHost(logger, IPAddress.Any, port, networkEventBus);

    messageHandler.SendingMessage += (string message) =>
    {
        Task.Run(async () =>
        {
            await host.BroadcastAsync(message);
        });
    };

    return host;
});

builder.Services.AddSingleton<INetworkClient>((sp) =>
{
    ILogger<TcpAppClient> logger = sp.GetRequiredService<ILogger<TcpAppClient>>();
    INetworkEventBus networkEventBus = sp.GetRequiredService<INetworkEventBus>();
    MessageHandler messageHandler = sp.GetRequiredService<MessageHandler>();

    TcpAppClient client = new(logger, port, GetLocalIPAddress(), networkEventBus);

    messageHandler.SendingMessage += (string message) =>
    {
        Task.Run(async () =>
        {
            await client.WriteAsync(message);
        });
    };

    return client;
});

builder.Services.AddSingleton<ISSEHandler>((sp) =>
{
    ILogger<SSEHandler> logger = sp.GetRequiredService<ILogger<SSEHandler>>();
    MessageHandler messageHandler = sp.GetRequiredService<MessageHandler>();
    ActionHandler actionHandler = sp.GetRequiredService<ActionHandler>();

    SSEHandler sseHandler = new SSEHandler(logger);

    messageHandler.ReceivedMessage += (string message) => sseHandler.AddEvent(message);
    actionHandler.ReceivedAction += (string action) => sseHandler.AddEvent(action);

    return sseHandler;
});

builder.Services.AddSingleton<IHostFacade, HostFacade>();
builder.Services.AddSingleton<IClientFacade, ClientFacade>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.MapFallbackToFile("/index.html");

if (HybridSupport.IsElectronActive)
{
    Task.Run(async () =>
    {
        var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
        {
            Width = 1200,
            Height = 800
        });
        window.OnClosed += () => Electron.App.Quit();
    });
}

app.Run();

Log.CloseAndFlush();
