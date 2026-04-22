using GongSolutions.Wpf.DragDrop;
using StreamBoard.Core;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Actions.Views.Components.Editor;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class ActionListViewModel : ObservableObject, IDropTarget
    {
        private readonly Action _onSaveRequested;

        private ObservableCollection<BaseAction>? _actions;
        public ObservableCollection<BaseAction>? Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        public ObservableCollection<ActionFieldViewModel> ActionFields { get; } = [];

        private bool _isActionDialogOpen;
        public bool IsActionDialogOpen
        {
            get => _isActionDialogOpen;
            set => SetProperty(ref _isActionDialogOpen, value);
        }

        private BaseAction? _originalActionToEdit;
        private BaseAction? _editingActionCopy;
        public BaseAction? EditingActionCopy
        {
            get => _editingActionCopy;
            set => SetProperty(ref _editingActionCopy, value);
        }

        public ICommand DeleteActionCommand { get; }
        public ICommand ClearActionsCommand { get; }
        public ICommand OpenEditDialogCommand { get; }
        public ICommand ReceiveActionDropCommand { get; }

        public ActionListViewModel(Action onSaveRequested)
        {
            _onSaveRequested = onSaveRequested;

            DeleteActionCommand = new RelayCommand<string>(id =>
            {
                if (string.IsNullOrEmpty(id) || Actions == null) return;

                var actionToRemove = Actions.FirstOrDefault(action => action.Id == id);
                if (actionToRemove != null)
                {
                    Actions.Remove(actionToRemove);
                    _onSaveRequested.Invoke();
                }
            });

            ClearActionsCommand = new RelayCommand(_ =>
            {
                if (Actions != null && Actions.Count > 0)
                {
                    Actions.Clear();
                    _onSaveRequested.Invoke();
                }
            });

            OpenEditDialogCommand = new RelayCommand<string>(id =>
            {
                if (Actions == null) return;

                _originalActionToEdit = Actions.FirstOrDefault(a => a.Id == id);

                if (_originalActionToEdit != null)
                {
                    EditingActionCopy = _originalActionToEdit.Copy();
                    GenerateFieldsForAction(EditingActionCopy);

                    var dialog = new ActionEditWindow
                    {
                        DataContext = this,
                        Owner = Application.Current.MainWindow
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        int index = Actions.IndexOf(_originalActionToEdit);
                        if (index >= 0)
                        {
                            Actions[index] = EditingActionCopy;
                            _onSaveRequested.Invoke();
                        }
                    }

                    CloseDialog();
                }
            });

            ReceiveActionDropCommand = new RelayCommand<object>(payload =>
            {
                Actions ??= [];

                if (payload is ActionDescriptor descriptor)
                {
                    var newAction = descriptor.CreateInstance();
                    Actions.Add(newAction);
                    _onSaveRequested.Invoke();
                }
            });
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data == null) return;

            if (dropInfo.DragInfo?.SourceCollection != null &&
                dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
            else if (dropInfo.TargetCollection is ObservableCollection<BaseAction>)
            {
                dropInfo.Effects = DragDropEffects.Move;
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
                    _onSaveRequested.Invoke();
                }
            }
        }

        private void CloseDialog()
        {
            IsActionDialogOpen = false;
            _originalActionToEdit = null;
            EditingActionCopy = null;
            ActionFields.Clear();
        }

        private void GenerateFieldsForAction(BaseAction action)
        {
            ActionFields.Clear();
            var fields = ActionFieldsGenerator.GenerateFields(action);
            foreach (var field in fields)
            {
                ActionFields.Add(field);
            }
        }
    }
}