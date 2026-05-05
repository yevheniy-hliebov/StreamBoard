using StreamTabula.Components.Dialogs;
using System.Windows;

namespace StreamTabula.Core.Services
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string title, string message);
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
    }
}
