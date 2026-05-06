using StreamTabula.Components.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace StreamTabula.Components.Controls
{
    public partial class LabeledToggle : UserControl
    {
        public LabeledToggle()
        {
            InitializeComponent();
        }

        public string ToggleOffText
        {
            get { return (string)GetValue(ToggleOffTextProperty); }
            set { SetValue(ToggleOffTextProperty, value); }
        }
        public static readonly DependencyProperty ToggleOffTextProperty =
            DependencyProperty.Register("ToggleOffText", typeof(string), typeof(LabeledToggle), new PropertyMetadata("Off"));

        public string ToggleOnText
        {
            get { return (string)GetValue(ToggleOnTextProperty); }
            set { SetValue(ToggleOnTextProperty, value); }
        }
        public static readonly DependencyProperty ToggleOnTextProperty =
            DependencyProperty.Register("ToggleOnText", typeof(string), typeof(LabeledToggle), new PropertyMetadata("On"));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(LabeledToggle),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
