using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Controls;

public partial class DropdownSelector : UserControl
{

    public DropdownSelector()
    {
        InitializeComponent();
    }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DropdownSelector), new PropertyMetadata(null));


    public DropdownOption SelectedItem
    {
        get => (DropdownOption)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(DropdownOption), typeof(DropdownSelector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(DropdownSelector), new PropertyMetadata("Select scene..."));

    public object IconName
    {
        get => GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }
    
    public static readonly DependencyProperty IconNameProperty =
        DependencyProperty.Register(nameof(IconName), typeof(object), typeof(DropdownSelector), new PropertyMetadata("FitPage24"));

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.DataContext is DropdownOption selectedModel)
        {
            SelectedItem = selectedModel;
        }
    }
}
