using StreamTabula.Core.Services;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Variables.Services;

namespace StreamTabula.Features.Decks.Services
{
    public interface IDeckButtonService
    {
        public BaseCanvasConfig CanvasConfig { get; }

        public event Action<int, DeckButtonConfig>? ButtonAppearanceChanged;

        public Dictionary<string, DeckButtonConfig> GetCurrentButtonMap();
        public DeckButtonConfig GetOrCreateButton(int index);
        
        public Task ExecuteButtonActions(DeckButtonConfig config);
        
        public event Action<int, int>? ButtonsSwapped;
        public event Action<int, DeckButtonConfig>? ButtonChanged;
        public void SwapButtons(int sourceIndex, int targetIndex);

        void CopyButton(int index);
        void PasteButton(int index);
        void CutButton(int index);
        void DeleteButton(int index);
        bool CanPaste();

        void AddActionToButton(int index, ActionDescriptor descriptor);
        public void SaveChanges();

    }

    public class DeckButtonService : IDeckButtonService
    {
        private readonly DeckStorage _storage;
        private readonly DeckProfile _profile;
        private readonly IClipboardService _clipboardService;
        private readonly IVariableService _variableService;

        public event Action<int, int>? ButtonsSwapped;
        public event Action<int, DeckButtonConfig>? ButtonChanged;

        public event Action<int, DeckButtonConfig>? ButtonAppearanceChanged;

        public DeckButtonService(DeckStorage storage, IClipboardService clipboardService, IVariableService variableService)
        {
            _storage = storage;
            _profile = _storage.Current;
            _clipboardService = clipboardService;
            _variableService = variableService;
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

            foreach (var kvp in map)
            {
                var config = kvp.Value;
                config.Initialize(_variableService);

                config.PropertyChanged -= Config_PropertyChanged;
                config.PropertyChanged += Config_PropertyChanged;
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
                config.Initialize(_variableService);

                config.PropertyChanged += Config_PropertyChanged;

                map[key] = config;
                _storage.Save();
            }
            return config;
        }

        public async Task ExecuteButtonActions(DeckButtonConfig config)
        {
            if (config?.Actions == null) return;

            var context = new ActionExecutionContext();

            foreach (var action in config.Actions.ToList())
            {
                try { await action.ExecuteAsync(context); }
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

        public void CopyButton(int index)
        {
            var map = GetCurrentButtonMap();
            if (map.TryGetValue(index.ToString(), out var config))
            {
                _clipboardService.Copy(config);
            }
        }

        public void CutButton(int index)
        {
            var map = GetCurrentButtonMap();
            if (map.TryGetValue(index.ToString(), out var config))
            {
                _clipboardService.Cut(config);
            }

            if (map.Remove(index.ToString()))
            {
                _storage.Save();
                ButtonChanged?.Invoke(index, new DeckButtonConfig());
            }
        }

        public void PasteButton(int index)
        {
            var newConfig = _clipboardService.Paste<DeckButtonConfig>();
            if (newConfig == null) return;

            newConfig.Initialize(_variableService);

            var map = GetCurrentButtonMap();
            map[index.ToString()] = newConfig;

            _storage.Save();
            ButtonChanged?.Invoke(index, newConfig);
        }

        public void DeleteButton(int index)
        {
            var map = GetCurrentButtonMap();
            if (map.Remove(index.ToString()))
            {
                _storage.Save();
            }
        }

        public bool CanPaste()
        {
            return _clipboardService.HasDataOfType<DeckButtonConfig>();
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

        private void Config_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is DeckButtonConfig config)
            {
                if (e.PropertyName is nameof(DeckButtonConfig.DisplayImage) or
                    nameof(DeckButtonConfig.Name) or
                    nameof(DeckButtonConfig.BackgroundColor))
                {
                    var map = GetCurrentButtonMap();
                    var item = map.FirstOrDefault(kvp => kvp.Value == config);

                    if (item.Key != null && int.TryParse(item.Key, out int index))
                    {
                        ButtonAppearanceChanged?.Invoke(index, config);
                    }
                }
            }
        }
    }
}
