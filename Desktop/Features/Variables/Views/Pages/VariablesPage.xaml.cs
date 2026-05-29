using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Variables.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Variables.Views.Pages
{
    public partial class VariablesPage : Page
    {
        public VariablesPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<VariablesViewModel>();
        }
    }
}
