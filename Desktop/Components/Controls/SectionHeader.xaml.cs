using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Controls
{
    public partial class SectionHeader : UserControl
    {
        public SectionHeader() => InitializeComponent();

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SectionHeader), new PropertyMetadata("Section Header"));
    }
}