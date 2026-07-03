using System.Windows;

namespace StreamTabula.Components.Dialogs;

public partial class ConfirmationDialogWindow : BaseDialog
{
    public string Message { get; }

    public ConfirmationDialogWindow(string title, string message)
    {
        InitializeComponent();

        Title = title;
        Message = message;

        DataContext = this;
    }

    //private void YesButton_Click(object sender, RoutedEventArgs e)
    //{
    //    Submit(sender, e);
    //}

    //private void NoButton_Click(object sender, RoutedEventArgs e)
    //{
    //    Cancel(sender, e);
    //}
}
