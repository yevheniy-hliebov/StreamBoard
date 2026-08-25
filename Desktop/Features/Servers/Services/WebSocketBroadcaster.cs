using StreamTabula.Features.Servers.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace StreamTabula.Features.Servers.Services;

public interface IWebSocketBroadcaster
{
    void AddClient(WebSocket socket);
    Task BroadcastAsync(WebsocketMessageType type, object data);
}

public class WebSocketBroadcaster : IWebSocketBroadcaster
{
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void AddClient(WebSocket socket)
    {
        string id = Guid.NewGuid().ToString();
        _clients.TryAdd(id, socket);
    }

    public async Task BroadcastAsync(WebsocketMessageType type, object data)
    {
        var message = new WebsocketMessage(type, data);
        string json = JsonSerializer.Serialize(message, _jsonOptions);

        var buffer = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(buffer);

        var broadcastTasks = _clients.Select(async client =>
        {
            var (id, socket) = client;

            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception)
                {
                    _clients.TryRemove(id, out _);
                }
            }
            else
            {
                _clients.TryRemove(id, out _);
            }
        });

        await Task.WhenAll(broadcastTasks);
    }
}
