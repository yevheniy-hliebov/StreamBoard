using StreamBoard.Core;
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
                    if (_selectedButton.Config != null)
                    {
                        _selectedButton.Config.PropertyChanged -= OnConfigPropertyChanged;
                    }
                }

                if (SetProperty(ref _selectedButton, value))
                {
                    if (_selectedButton != null)
                    {
                        _selectedButton.IsSelected = true;

                        if (_selectedButton.Config == null)
                        {
                            var newConfig = new DeckButtonConfig();
                            _selectedButton.Config = newConfig;

                            var map = _storage.CurrentProfile.CurrentPageButtonMap;
                            if (map != null)
                            {
                                map[_selectedButton.Index.ToString()] = newConfig;
                            }
                        }

                        _selectedButton.Config.PropertyChanged += OnConfigPropertyChanged;
                    }
                }
            }
        }

        public DeckCanvasViewModel(GridDeckStorage storage)
        {
            _storage = storage;
            CanvasConfig = storage.CurrentProfile.CanvasConfig;

            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;

            SelectButtonCommand = new RelayCommand(async p =>
            {
                if (p is not DeckButtonSlot slot) return;

                if (!IsClickMode)
                {
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
            var map = _storage.CurrentProfile.CurrentPageButtonMap;

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

        private void OnConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckButtonConfig.Name)
                || e.PropertyName == nameof(DeckButtonConfig.ImagePath)
                || e.PropertyName == nameof(DeckButtonConfig.BackgroundColor))
            {
                _storage.Save();
            }
        }

        private void HandleDrop(object payload, DeckButtonSlot target)
        {
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

                var map = _storage.CurrentProfile.CurrentPageButtonMap;
                if (map != null)
                {
                    map[target.Index.ToString()] = newConfig;
                }
            }

            var newActionInstance = descriptor.CreateInstance();
            target.Config.Actions.Add(newActionInstance);
            _storage.Save();
        }

        private void SwapButtons(DeckButtonSlot source, DeckButtonSlot target)
        {
            if (source == null || target == null || source == target) return;

            SelectedButton = null;

            (source.Config, target.Config) = (target.Config, source.Config);

            var map = _storage.CurrentProfile.CurrentPageButtonMap;
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
            }
        }
    }
}