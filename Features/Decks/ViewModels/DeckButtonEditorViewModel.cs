using GongSolutions.Wpf.DragDrop;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class DeckButtonEditorViewModel : ObservableObject, IDropTarget
    {
        private readonly GridDeckStorage _storage;

        private DeckButtonSlot? _editingSlot;
        public DeckButtonSlot? EditingSlot
        {
            get => _editingSlot;
            set => SetProperty(ref _editingSlot, value);
        }

        public ICommand ClearButtonCommand { get; }
        public ICommand DeleteActionCommand { get; }
        public ICommand ClearActionsCommand { get; }

        public DeckButtonEditorViewModel(GridDeckStorage storage)
        {
            _storage = storage;

            ClearButtonCommand = new RelayCommand(_ =>
            {
                if (EditingSlot?.Config != null)
                {
                    EditingSlot.Config.ResetAppearance();
                    _storage.Save();
                }
            });

            DeleteActionCommand = new RelayCommand<string>(id =>
            {
                if (string.IsNullOrEmpty(id) || EditingSlot?.Config?.Actions == null) return;

                var actionToRemove = EditingSlot.Config.Actions.FirstOrDefault(action => action.Id == id);
                if (actionToRemove != null)
                {
                    EditingSlot.Config.Actions.Remove(actionToRemove);
                    _storage.Save();
                }
            });

            ClearActionsCommand = new RelayCommand(_ =>
            {
                if (EditingSlot?.Config?.Actions != null)
                {
                    EditingSlot.Config.Actions.Clear();
                    _storage.Save();
                }
            });
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
