using System.Windows;
using Wpf.Ui.Controls;

namespace StreamTabula.Features.Variables.Views.Components
{
    public partial class AddVariableDialogWindow : FluentWindow
    {
        public string VariableName { get; private set; } = string.Empty;
        public string VariableValue { get; private set; } = string.Empty;

        public AddVariableDialogWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => NameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                return;
            }

            VariableName = NameTextBox.Text.Trim();
            VariableValue = ValueTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}