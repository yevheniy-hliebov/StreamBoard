using StreamTabula.Components.Dialogs;
using StreamTabula.Features.Variables.Views.Components;
using System.Windows;

namespace StreamTabula.Core.Services
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task<(string Name, string Value)?> ShowAddVariableDialogAsync();
    }

    public class DialogService : IDialogService
    {
        public Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new ConfirmationDialogWindow(title, message)
                {
                    Owner = Application.Current.MainWindow
                };

                bool result = dialog.ShowDialog() == true;
                tcs.SetResult(result);
            });

            return tcs.Task;
        }

        public Task<(string Name, string Value)?> ShowAddVariableDialogAsync()
        {
            var tcs = new TaskCompletionSource<(string, string)?>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new AddVariableDialogWindow
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() == true)
                {
                    tcs.SetResult((dialog.VariableName, dialog.VariableValue));
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }
    }
}