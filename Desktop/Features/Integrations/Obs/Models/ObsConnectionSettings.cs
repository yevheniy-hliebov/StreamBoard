using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.OBS.Models;

public class OBSConnectionSettings
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = "localhost";

    [JsonPropertyName("port")]
    public int Port { get; set; } = 4455;

    private string? _password;

    [JsonPropertyName("password")]
    public string? Password
    {
        get => _password;
        set => _password = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [JsonPropertyName("auto_reconnect")]
    public bool AutoReconnect { get; set; } = true;

    [JsonPropertyName("auto_connect_startup")]
    public bool AutoConnectOnStartup { get; set; } = true;

    private int _reconnectDelay = 30;

    [JsonPropertyName("reconnect_delay")]
    public int ReconnectDelay
    {
        get => _reconnectDelay;
        set => _reconnectDelay = Math.Clamp(value, 15, 300);
    }

    private int _keepAliveIntervalSeconds = 30;

    [JsonPropertyName("keep_alive_interval")]
    public int KeepAliveIntervalSeconds
    {
        get => _keepAliveIntervalSeconds;
        set => _keepAliveIntervalSeconds = Math.Clamp(value, 5, 300);
    }
}