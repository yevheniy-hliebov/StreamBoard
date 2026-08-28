using GongSolutions.Wpf.DragDrop;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Actions.Views.Editor;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Actions.ViewModels;

public class ActionListViewModel : ObservableObject, IDropTarget
{
    private readonly IActionCollectionService _actionService;
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

    private BaseAction? _selectedAction;
    public BaseAction? SelectedAction
    {
        get => _selectedAction;
        set => SetProperty(ref _selectedAction, value);
    }

    public IRelayCommand<string> DeleteActionCommand { get; }
    public IRelayCommand<object?> ClearActionsCommand { get; }
    public IRelayCommand<string> OpenEditDialogCommand { get; }
    public IRelayCommand<object> ReceiveActionDropCommand { get; }
    public IRelayCommand<string> CopyActionCommand { get; }
    public IRelayCommand<string> CutActionCommand { get; }
    public IRelayCommand<object> PasteActionCommand { get; }
    public IRelayCommand<string> DuplicateActionCommand { get; }

    public ActionListViewModel(Action onSaveRequested, IActionCollectionService actionService)
    {
        _actionService = actionService;
        _onSaveRequested = onSaveRequested;

        DeleteActionCommand = new RelayCommand<string>(id =>
        {
            if (string.IsNullOrEmpty(id) || Actions == null) return;
            _actionService.RemoveAction(Actions, id);
            _onSaveRequested.Invoke();
        });

        ClearActionsCommand = new RelayCommand<object?>(async _ =>
        {
            if (Actions != null && await _actionService.TryClearActionsAsync(Actions))
            {
                _onSaveRequested.Invoke();
            }
        });

        CopyActionCommand = new RelayCommand<string>(id =>
        {
            if (string.IsNullOrEmpty(id) || Actions == null) return;
            _actionService.CopyAction(Actions, id);
        });

        CutActionCommand = new RelayCommand<string>(id =>
        {
            if (string.IsNullOrEmpty(id) || Actions == null) return;
            _actionService.CutAction(Actions, id);
            _onSaveRequested.Invoke();
        });

        PasteActionCommand = new RelayCommand<object>(_ =>
        {
            Actions ??= [];
            _actionService.PasteAction(Actions);
            _onSaveRequested.Invoke();
        });

        DuplicateActionCommand = new RelayCommand<string>(id =>
        {
            if (string.IsNullOrEmpty(id) || Actions == null) return;
            _actionService.DuplicateAction(Actions, id);
            _onSaveRequested.Invoke();
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
                    _actionService.UpdateAction(Actions, _originalActionToEdit, EditingActionCopy);
                    _onSaveRequested.Invoke();
                }

                CloseDialog();
            }
        });

        ReceiveActionDropCommand = new RelayCommand<object>(payload =>
        {
            Actions ??= [];

            if (payload is ActionDescriptor descriptor)
            {
                _actionService.AddAction(Actions, descriptor);
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

            if (oldIndex != newIndex && Actions != null)
            {
                _actionService.MoveAction(Actions, oldIndex, newIndex);
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