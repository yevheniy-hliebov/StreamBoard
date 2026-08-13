using StreamTabula.Controls.Icons;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Actions.Views.Components.Editor;

public partial class ActionListItem : UserControl
{
    public ActionListItem() => InitializeComponent();

    public FluentIconType Icon
    {
        get { return (FluentIconType)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(FluentIconType), typeof(ActionListItem), new PropertyMetadata(FluentIconType.Checkbox));


    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ActionListItem), new PropertyMetadata(string.Empty));


    public ICommand DeleteCommand
    {
        get { return (ICommand)GetValue(DeleteCommandProperty); }
        set { SetValue(DeleteCommandProperty, value); }
    }

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ActionListItem), new PropertyMetadata(null));


    public string ActionId
    {
        get { return (string)GetValue(ActionIdProperty); }
        set { SetValue(ActionIdProperty, value); }
    }

    public static readonly DependencyProperty ActionIdProperty =
        DependencyProperty.Register(nameof(ActionId), typeof(string), typeof(ActionListItem), new PropertyMetadata(null));
}
