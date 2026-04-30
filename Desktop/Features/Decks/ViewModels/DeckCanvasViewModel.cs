using StreamBoard.Core;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class DeckCanvasViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;

        public GridCanvasConfig CanvasConfig { get; }
        public ObservableCollection<DeckButtonSlot> Buttons { get; } = [];

        private bool _isClickMode;
        public bool IsClickMode
        {
            get => _isClickMode;
            set
            {
                if (SetProperty(ref _isClickMode, value))
                {
                    if (_isClickMode) SelectedButton = null;
                }
            }
        }

        public ICommand SelectButtonCommand { get; }

        private DeckButtonSlot? _selectedButton;
        public DeckButtonSlot? SelectedButton
        {
            get => _selectedButton;
            set
            {
                if (_selectedButton != null)
                {
                    _selectedButton.IsSelected = false;
                    _selectedButton.Config?.PropertyChanged -= OnConfigPropertyChanged;
                }

                if (value != null && value.Config == null)
                {
                    var newConfig = new DeckButtonConfig();
                    value.Config = newConfig;

                    var map = _storage.Current.CurrentPageButtonMap;
                    if (map != null)
                    {
                        map[value.Index.ToString()] = newConfig;
                    }
                }

                if (SetProperty(ref _selectedButton, value))
                {
                    if (_selectedButton != null)
                    {
                        _selectedButton.IsSelected = true;

                        _selectedButton.Config?.PropertyChanged += OnConfigPropertyChanged;
                    }
                }
            }
        }

        public DeckCanvasViewModel(GridDeckStorage storage)
        {
            _storage = storage;
            CanvasConfig = storage.Current.CanvasConfig;

            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;

            SelectButtonCommand = new RelayCommand(async p =>
            {
                if (p is not DeckButtonSlot slot) return;

                if (!IsClickMode)
                {
                    if (SelectedButton == slot) return;
                    SelectedButton = slot;
                }
                else
                {
                    if (slot.Config?.Actions != null)
                    {
                        foreach (var action in slot.Config.Actions)
                        {
                            try
                            {
                                await action.ExecuteAsync();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            });

            RebuildButtons();
        }

        public void RebuildButtons()
        {
            SelectedButton = null;
            Buttons.Clear();
            var map = _storage.Current.CurrentPageButtonMap;

            foreach (var index in CanvasConfig.Cells)
            {
                DeckButtonConfig? config = null;
                if (map != null && map.TryGetValue(index.ToString(), out var btn))
                {
                    config = btn;
                }

                var slot = new DeckButtonSlot(index, config, HandleDrop);
                Buttons.Add(slot);
            }
        }

        private void OnCanvasConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridCanvasConfig.SelectedGrid))
            {
                RebuildButtons();
                _storage.Save();
            }
        }

        public event Action<int, DeckButtonConfig>? ButtonAppearanceChanged;

        private void OnConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckButtonConfig.Name)
                || e.PropertyName == nameof(DeckButtonConfig.ImagePath)
                || e.PropertyName == nameof(DeckButtonConfig.BackgroundColor))
            {
                _storage.Save();

                if (sender is DeckButtonConfig config)
                {
                    var slot = Buttons.FirstOrDefault(b => b.Config == config);
                    if (slot != null)
                    {
                        ButtonAppearanceChanged?.Invoke(slot.Index, config);
                    }
                }
            }
        }

        private void HandleDrop(object payload, DeckButtonSlot target)
        {
            if (IsClickMode) return;

            if (payload is DeckButtonSlot sourceSlot)
            {
                SwapButtons(sourceSlot, target);
            }
            else if (payload is ActionDescriptor descriptor)
            {
                AddActionToSlot(descriptor, target);
            }
        }

        private void AddActionToSlot(ActionDescriptor descriptor, DeckButtonSlot target)
        {
            if (target.Config == null)
            {
                var newConfig = new DeckButtonConfig();
                target.Config = newConfig;

                var map = _storage.Current.CurrentPageButtonMap;
                map?[target.Index.ToString()] = newConfig;
            }

            var newActionInstance = descriptor.CreateInstance();
            target.Config.Actions.Add(newActionInstance);
            _storage.Save();
        }

        public event Action<int, int>? ButtonsSwapped;

        private void SwapButtons(DeckButtonSlot source, DeckButtonSlot target)
        {
            if (source == null || target == null || source == target) return;

            SelectedButton = null;

            (source.Config, target.Config) = (target.Config, source.Config);

            var map = _storage.Current.CurrentPageButtonMap;
            if (map != null)
            {
                if (source.Config != null)
                    map[source.Index.ToString()] = source.Config;
                else
                    map.Remove(source.Index.ToString());

                if (target.Config != null)
                    map[target.Index.ToString()] = target.Config;
                else
                    map.Remove(target.Index.ToString());

                _storage.Save();

                ButtonsSwapped?.Invoke(source.Index, target.Index);
            }
        }
    }
}