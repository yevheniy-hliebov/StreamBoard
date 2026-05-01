using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Services
{
    public interface IDeckButtonService
    {
        public BaseCanvasConfig CanvasConfig { get; }
        public Dictionary<string, DeckButtonConfig> GetCurrentButtonMap();
        public DeckButtonConfig GetOrCreateButton(int index);
        public Task ExecuteButtonActions(DeckButtonConfig config);
        public void SwapButtons(int sourceIndex, int targetIndex);
        void AddActionToButton(int index, ActionDescriptor descriptor);
        public void SaveChanges();

        public event Action<int, int>? ButtonsSwapped;
    }

    public class DeckButtonService : IDeckButtonService
    {
        private readonly DeckStorage _storage;
        private readonly DeckProfile _profile;

        public event Action<int, DeckButtonConfig>? ButtonAppearanceChanged;
        public event Action<int, int>? ButtonsSwapped;

        public DeckButtonService(DeckStorage storage)
        {
            _storage = storage;
            _profile = _storage.Current;
        }

        public BaseCanvasConfig CanvasConfig => _profile.CanvasConfig;

        public Dictionary<string, DeckButtonConfig> GetCurrentButtonMap()
        {
            var selectedId = _profile.PagesState.SelectedPageId;
            if (!_profile.ButtonMaps.TryGetValue(selectedId, out var map))
            {
                map = new Dictionary<string, DeckButtonConfig>();
                _profile.ButtonMaps[selectedId] = map;
            }
            return map;
        }

        public DeckButtonConfig GetOrCreateButton(int index)
        {
            var map = GetCurrentButtonMap();
            string key = index.ToString();

            if (!map.TryGetValue(key, out var config))
            {
                config = new DeckButtonConfig();
                map[key] = config;
                _storage.Save();
            }
            return config;
        }

        public async Task ExecuteButtonActions(DeckButtonConfig config)
        {
            if (config?.Actions == null) return;

            foreach (var action in config.Actions.ToList())
            {
                try { await action.ExecuteAsync(); }
                catch { /* Error logging */ }
            }
        }

        public void SwapButtons(int sourceIndex, int targetIndex)
        {
            var map = GetCurrentButtonMap();
            string srcKey = sourceIndex.ToString();
            string tgtKey = targetIndex.ToString();

            map.TryGetValue(srcKey, out var srcConfig);
            map.TryGetValue(tgtKey, out var tgtConfig);

            if (tgtConfig != null) map[srcKey] = tgtConfig; else map.Remove(srcKey);
            if (srcConfig != null) map[tgtKey] = srcConfig; else map.Remove(tgtKey);

            _storage.Save();
            ButtonsSwapped?.Invoke(sourceIndex, targetIndex);
        }

        public void AddActionToButton(int index, ActionDescriptor descriptor)
        {
            var config = GetOrCreateButton(index);
            config.Actions.Add(descriptor.CreateInstance());
            _storage.Save();
        }

        public void SaveChanges()
        {
            _storage.Save();
        }
    }
}
