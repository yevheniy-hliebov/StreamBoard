using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace StreamBoard
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Mica;
        }
    }
}