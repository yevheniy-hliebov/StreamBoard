using StreamTabula.Features.Actions.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Features.Actions.Views.Editor;

public partial class ActionListView : UserControl
{
    public ActionListView() => InitializeComponent();

    public ActionListViewModel? ViewModel
    {
        get => (ActionListViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(ActionListViewModel),
            typeof(ActionListView),
            new PropertyMetadata(null));
}