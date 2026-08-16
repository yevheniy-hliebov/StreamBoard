using System.Text.Json.Serialization;

namespace StreamTabula.Core.Models;

public interface IVersionedConfig
{
    int ConfigVersion { get; set; }
}
