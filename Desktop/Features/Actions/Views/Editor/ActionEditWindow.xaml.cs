using System.Windows;
using Wpf.Ui.Controls;

namespace StreamTabula.Features.Actions.Views.Editor;

public partial class ActionEditWindow : FluentWindow
{
    public ActionEditWindow()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}