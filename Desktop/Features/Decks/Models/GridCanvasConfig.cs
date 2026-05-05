using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class GridCanvasConfig : BaseCanvasConfig
    {
        public GridCanvasConfig() : base(DeckType.Grid) { }

        private GridLayout _selectedGrid = GridLayout.Grids[0];

        [JsonPropertyName("selected_grid")]
        public GridLayout SelectedGrid
        {
            get => _selectedGrid;
            set
            {
                if (SetProperty(ref _selectedGrid, value))
                {
                    OnPropertyChanged(nameof(Cells));
                }
            }
        }

        [JsonIgnore]
        public List<GridLayout> Grids => GridLayout.Grids;

        [JsonIgnore]
        public IEnumerable<int> Cells =>
            Enumerable.Range(0, SelectedGrid.Columns * SelectedGrid.Rows);
    }
}
