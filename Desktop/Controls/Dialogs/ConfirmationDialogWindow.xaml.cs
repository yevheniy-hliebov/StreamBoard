namespace StreamTabula.Controls.Dialogs;

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
}
