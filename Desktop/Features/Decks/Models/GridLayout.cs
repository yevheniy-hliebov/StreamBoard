using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class GridLayout(string name, int columns, int rows)
    {
        [JsonPropertyName("name")]
        public string Name { get; } = name;

        [JsonPropertyName("columns")]
        public int Columns { get; } = columns;

        [JsonPropertyName("rows")]
        public int Rows { get; } = rows;

        public override bool Equals(object? obj) =>
            obj is GridLayout other && Name == other.Name && Columns == other.Columns && Rows == other.Rows;

        public override int GetHashCode() => HashCode.Combine(Name, Columns, Rows);

        public static List<GridLayout> Grids { get; } = [
            new("3x2", 3, 2),
            new("4x4", 4, 4),
            new("6x3", 6, 3),
            new("7x4", 7, 4),
            new("8x4", 8, 4),
        ];
    }
}
