using Microsoft.Win32;
using StreamTabula.Features.Actions.Models;
using System.Reflection;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Actions.ViewModels;

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

    public IRelayCommand<object?> BrowseCommand => new RelayCommand<object?>(_ => Browse());

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