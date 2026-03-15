using GongSolutions.Wpf.DragDrop;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Wpf.Ui.Input;


namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject, IDropTarget
    {
        private readonly GridDeckStorage _storage;

        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }

        public GridCanvasConfig CanvasConfig { get; }

        public ObservableCollection<DeckButtonSlot> Buttons { get; } = [];

        public ICommand SelectButtonCommand { get; }
        public ICommand ClearButtonCommand { get; }
        public ICommand DeleteActionCommand { get; }
        public ICommand ClearActionsCommand { get; }

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

            DeleteActionCommand = new RelayCommand<string>(DeleteActionInSelectedButton);

            ClearActionsCommand = new RelayCommand(_ =>
            {
                if (SelectedButton?.Config?.Actions != null)
                {
                    SelectedButton.Config.Actions.Clear();
                    _storage.Save();
                }
            }, _ => SelectedButton?.Config?.Actions?.Count > 0);
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

                var slot = new DeckButtonSlot(index, config, HandleDrop);
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

            var tempConfig = source.Config;
            source.Config = target.Config;
            target.Config = tempConfig;

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

        private void DeleteActionInSelectedButton(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var selectedButton = SelectedButton;

            if (selectedButton?.Config?.Actions == null)
                return;

            var actionToRemove = selectedButton.Config.Actions.FirstOrDefault(action => action.Id == id);

            if (actionToRemove != null)
            {
                selectedButton.Config.Actions.Remove(actionToRemove);
            }

            _storage.Save();
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data == null) return;

            if (dropInfo.DragInfo?.SourceCollection != null &&
                dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
            else if (dropInfo.TargetCollection is ObservableCollection<DeckAction>)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo?.SourceCollection == dropInfo.TargetCollection && dropInfo.Data != null)
            {
                var list = (System.Collections.IList)dropInfo.DragInfo.SourceCollection;

                int oldIndex = list.IndexOf(dropInfo.Data);
                int newIndex = dropInfo.InsertIndex;

                if (oldIndex < newIndex) newIndex--;

                if (oldIndex != newIndex)
                {
                    dynamic observableCollection = dropInfo.TargetCollection;
                    observableCollection.Move(oldIndex, newIndex);
                    _storage.Save();
                }
            }
        }
    }
}