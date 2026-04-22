using Microsoft.Win32;
using StreamBoard.Core;
using StreamBoard.Features.Actions.Models;
using System.Reflection;
using System.Windows.Input;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class PathFieldViewModel(
        string label,
        string? hint,
        object targetAction,
        PropertyInfo property,
        PathSelectionType selectionType,
        string filter
    ) : ActionFieldViewModel(label, hint, targetAction, property)
    {
        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }

        public ICommand BrowseCommand => new RelayCommand(_ => Browse());

        private void Browse()
        {
            if (selectionType == PathSelectionType.File)
            {
                var dialog = new OpenFileDialog
                {
                    Title = Label,
                    Filter = filter,
                    FileName = Value
                };

                if (dialog.ShowDialog() == true)
                {
                    Value = dialog.FileName;
                }
            }
            else
            {
                var dialog = new OpenFolderDialog
                {
                    Title = Label,
                    InitialDirectory = string.IsNullOrWhiteSpace(Value) ? null : Value
                };

                if (dialog.ShowDialog() == true)
                {
                    Value = dialog.FolderName;
                }
            }
        }
    }
}