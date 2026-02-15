using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Servers.Components
{
    public partial class HttpRequestLogList : UserControl
    {
        public HttpRequestLogList() => InitializeComponent();

        public IEnumerable Logs
        {
            get => (IEnumerable)GetValue(LogsProperty);
            set => SetValue(LogsProperty, value);
        }
        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register(nameof(Logs), typeof(IEnumerable), typeof(HttpRequestLogList), new PropertyMetadata(null));
    }
}
