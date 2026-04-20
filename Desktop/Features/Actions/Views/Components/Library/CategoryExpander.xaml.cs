using StreamBoard.Core.Models;
using StreamBoard.Features.Integrations.Common.Models;
using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Actions.Views.Components.Library
{
    public partial class CategoryExpander : UserControl
    {
        public CategoryExpander() => InitializeComponent();

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CategoryExpander), new PropertyMetadata(string.Empty));

        public FluentIconType Icon
        {
            get => (FluentIconType)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(FluentIconType), typeof(CategoryExpander), new PropertyMetadata(FluentIconType.Folder));

        public IntegrationIconType? IntegrationIcon
        {
            get => (IntegrationIconType?)GetValue(IntegrationIconProperty);
            set => SetValue(IntegrationIconProperty, value);
        }
        public static readonly DependencyProperty IntegrationIconProperty =
            DependencyProperty.Register(nameof(IntegrationIcon), typeof(IntegrationIconType?), typeof(CategoryExpander), new PropertyMetadata(null));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CategoryExpander), new PropertyMetadata(true));

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(CategoryExpander), new PropertyMetadata(null));
    }
}
