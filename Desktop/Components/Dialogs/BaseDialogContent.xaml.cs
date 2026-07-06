using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Dialogs
{
    public partial class BaseDialogContent : UserControl
    {
        public BaseDialogContent()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(BaseDialogContent), new PropertyMetadata("BaseDialog"));

        public object DialogContent
        {
            get => GetValue(DialogContentProperty);
            set => SetValue(DialogContentProperty, value);
        }

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register(
                nameof(DialogContent),
                typeof(object),
                typeof(BaseDialogContent));

        public Thickness ContentPadding
        {
            get => (Thickness)GetValue(ContentPaddingProperty);
            set => SetValue(ContentPaddingProperty, value);
        }

        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register(
                nameof(ContentPadding),
                typeof(Thickness),
                typeof(BaseDialogContent),
                new PropertyMetadata(new Thickness(24)));


        public object Buttons
        {
            get => GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                nameof(Buttons),
                typeof(object),
                typeof(BaseDialogContent));

        public Thickness ButtonsPadding
        {
            get => (Thickness)GetValue(ButtonsPaddingProperty);
            set => SetValue(ButtonsPaddingProperty, value);
        }

        public static readonly DependencyProperty ButtonsPaddingProperty =
            DependencyProperty.Register(
                nameof(ButtonsPadding),
                typeof(Thickness),
                typeof(BaseDialogContent),
                new PropertyMetadata(new Thickness(24, 0, 24, 24)));
    }
}
