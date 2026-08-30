using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Features.Actions.Views.Library;

public partial class ActionLibraryView : UserControl
{
    public ActionLibraryView() => InitializeComponent();

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }
    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(ActionLibraryView), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public IEnumerable Categories
    {
        get => (IEnumerable)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }
    public static readonly DependencyProperty CategoriesProperty =
        DependencyProperty.Register(nameof(Categories), typeof(IEnumerable), typeof(ActionLibraryView), new PropertyMetadata(null));
}
