using StreamBoard.Core.Services;
using StreamBoard.Features.Actions.Models;
using System.Collections.ObjectModel;

namespace StreamBoard.Features.Actions.Services
{
    public interface IActionCollectionService
    {
        void AddAction(ObservableCollection<BaseAction> actions, ActionDescriptor descriptor);
        void RemoveAction(ObservableCollection<BaseAction> actions, string actionId);
        Task<bool> TryClearActionsAsync(ObservableCollection<BaseAction> actions);
        void UpdateAction(ObservableCollection<BaseAction> actions, BaseAction original, BaseAction updated);
        void MoveAction(ObservableCollection<BaseAction> actions, int oldIndex, int newIndex);
        void CopyAction(ObservableCollection<BaseAction> actions, string actionId);
        void CutAction(ObservableCollection<BaseAction> actions, string actionId);
        void PasteAction(ObservableCollection<BaseAction> actions, int insertIndex = -1);
        void DuplicateAction(ObservableCollection<BaseAction> actions, string actionId);
    }

    public class ActionCollectionService : IActionCollectionService
    {
        private readonly IDialogService _dialogService;
        private readonly IClipboardService _clipboardService;

        public ActionCollectionService(IDialogService dialogService, IClipboardService clipboardService)
        {
            _dialogService = dialogService;
            _clipboardService = clipboardService;
        }

        public void AddAction(ObservableCollection<BaseAction> actions, ActionDescriptor descriptor)
        {
            var newAction = descriptor.CreateInstance();
            actions.Add(newAction);
        }

        public void RemoveAction(ObservableCollection<BaseAction> actions, string actionId)
        {
            var actionToRemove = actions.FirstOrDefault(a => a.Id == actionId);
            if (actionToRemove != null)
            {
                actions.Remove(actionToRemove);
            }
        }

        public async Task<bool> TryClearActionsAsync(ObservableCollection<BaseAction> actions)
        {
            if (actions.Count == 0) return false;

            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                "Delete all actions",
                "Are you sure you want to delete all actions?");

            if (isConfirmed)
            {
                actions.Clear();
                return true;
            }

            return false;
        }

        public void UpdateAction(ObservableCollection<BaseAction> actions, BaseAction original, BaseAction updated)
        {
            int index = actions.IndexOf(original);
            if (index >= 0)
            {
                actions[index] = updated;
            }
        }

        public void MoveAction(ObservableCollection<BaseAction> actions, int oldIndex, int newIndex)
        {
            if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0 && oldIndex < actions.Count && newIndex <= actions.Count)
            {
                actions.Move(oldIndex, newIndex);
            }
        }

        public void CopyAction(ObservableCollection<BaseAction> actions, string actionId)
        {
            var actionToCopy = actions.FirstOrDefault(a => a.Id == actionId);
            if (actionToCopy != null)
            {
                _clipboardService.Copy(actionToCopy);
            }
        }

        public void CutAction(ObservableCollection<BaseAction> actions, string actionId)
        {
            var actionToCut = actions.FirstOrDefault(a => a.Id == actionId);
            if (actionToCut != null)
            {
                _clipboardService.Cut(actionToCut);
                actions.Remove(actionToCut);
            }
        }

        public void PasteAction(ObservableCollection<BaseAction> actions, int insertIndex = -1)
        {
            var pastedAction = _clipboardService.Paste<BaseAction>();
            if (pastedAction == null) return;

            var newAction = pastedAction.Copy();

            if (insertIndex >= 0 && insertIndex <= actions.Count)
            {
                actions.Insert(insertIndex, newAction);
            }
            else
            {
                actions.Add(newAction);
            }
        }

        public void DuplicateAction(ObservableCollection<BaseAction> actions, string actionId)
        {
            var original = actions.FirstOrDefault(a => a.Id == actionId);
            if (original == null) return;

            var duplicate = original.Copy();

            int index = actions.IndexOf(original);
            actions.Insert(index + 1, duplicate);
        }
    }
}
