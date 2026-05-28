using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Controls.ReorderList
{
    public partial class ReorderListItemWrapper : ContentControl
    {
        public ReorderListItemWrapper() => InitializeComponent();

        public bool ShowDragHandle
        {
            get => (bool)GetValue(ShowDragHandleProperty);
            set => SetValue(ShowDragHandleProperty, value);
        }
        public static readonly DependencyProperty ShowDragHandleProperty =
            DependencyProperty.Register(nameof(ShowDragHandle), typeof(bool), typeof(ReorderListItemWrapper), new PropertyMetadata(true));

        public bool UseDragHandle
        {
            get => (bool)GetValue(UseDragHandleProperty);
            set => SetValue(UseDragHandleProperty, value);
        }
        public static readonly DependencyProperty UseDragHandleProperty =
            DependencyProperty.Register(nameof(UseDragHandle), typeof(bool), typeof(ReorderListItemWrapper), new PropertyMetadata(true));
    }
}