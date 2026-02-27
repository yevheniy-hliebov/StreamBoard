using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;

        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }

        public GridCanvasConfig CanvasConfig { get; }

        public ObservableCollection<DeckButtonSlot> Buttons { get; } = [];

        public ICommand SelectButtonCommand { get; }
        public ICommand ClearButtonCommand { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            _storage = storage;
            
            Pages = new DeckPagesViewModel(storage);
            Library = new ActionLibraryViewModel(registry);
            CanvasConfig = storage.CurrentProfile.CanvasConfig;

            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;
            Pages.PropertyChanged += OnPagesPropertyChanged;

            RebuildButtons();

            SelectButtonCommand = new RelayCommand(p =>
            {
                if (p is DeckButtonSlot slot)
                    SelectedButton = slot;
            });

            ClearButtonCommand = new RelayCommand(_ =>
            {
                if (SelectedButton?.Config != null)
                {
                    SelectedButton.Config.ResetAppearance();
                }
            }, _ => SelectedButton?.Config != null);
        }

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

        private void OnCanvasConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridCanvasConfig.SelectedGrid))
            {
                RebuildButtons();
                _storage.Save();
            }
        }

        private void OnPagesPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pages.SelectedPage))
            {
                RebuildButtons();
                SelectedButton = null;
            }
        }

        private void RebuildButtons()
        {
            Buttons.Clear();

            var map = _storage.CurrentProfile.CurrentPageButtonMap;

            foreach (var index in CanvasConfig.Cells)
            {
                DeckButtonConfig? config = null;

                if (map != null && map.TryGetValue(index.ToString(), out var btn))
                {
                    config = btn;
                }

                // МАГІЯ ТУТ: Просто передаємо метод SwapButtons як третій параметр!
                var slot = new DeckButtonSlot(index, config, SwapButtons);

                Buttons.Add(slot);
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
        private void SwapButtons(DeckButtonSlot source, DeckButtonSlot target)
        {
            if (source == null || target == null || source == target) return;

            // Скидаємо виділення, щоб уникнути багів з підпискою OnConfigPropertyChanged
            SelectedButton = null;

            // Свапаємо конфіги місцями
            var tempConfig = source.Config;
            source.Config = target.Config;
            target.Config = tempConfig;

            // Оновлюємо збереження (Map)
            var map = _storage.CurrentProfile.CurrentPageButtonMap;
            if (map != null)
            {
                // Оновлюємо джерело
                if (source.Config != null)
                    map[source.Index.ToString()] = source.Config;
                else
                    map.Remove(source.Index.ToString()); // Якщо кнопка стала порожньою

                // Оновлюємо ціль
                if (target.Config != null)
                    map[target.Index.ToString()] = target.Config;
                else
                    map.Remove(target.Index.ToString());

                // Зберігаємо профіль
                _storage.Save();
            }
        }
    }
}