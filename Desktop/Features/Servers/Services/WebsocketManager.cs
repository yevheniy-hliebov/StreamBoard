using StreamBoard.Features.Servers.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace StreamBoard.Features.Servers.Services
{
    public class WebsocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _clients = new();

        public void AddClient(WebSocket socket)
        {
            string id = Guid.NewGuid().ToString();
            _clients.TryAdd(id, socket);
        }

        public async Task BroadcastAsync(WebsocketMessageType type, object data)
        {
            var message = new WebsocketMessage(type, data);

            string json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var (id, socket) in _clients)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    _clients.TryRemove(id, out _);
                }
            }
        }
    }
}
