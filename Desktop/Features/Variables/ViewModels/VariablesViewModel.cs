using System.Collections.ObjectModel;
using System.Windows.Input;
using StreamTabula.Core;
using StreamTabula.Core.Services;
using StreamTabula.Features.Variables.Models;
using StreamTabula.Features.Variables.Services;

namespace StreamTabula.Features.Variables.ViewModels
{
    public class VariablesViewModel : ObservableObject
    {
        private readonly IVariableService _variableService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Variable> GlobalVariables => _variableService.GlobalVariables;
        public ObservableCollection<Variable> TemporaryVariables => _variableService.TemporaryVariables;

        public ICommand AddGlobalCommand { get; }
        public ICommand DeleteGlobalCommand { get; }
        public ICommand DeleteTemporaryCommand { get; }

        public VariablesViewModel(IVariableService variableService, IDialogService dialogService)
        {
            _variableService = variableService;
            _dialogService = dialogService;

            AddGlobalCommand = new RelayCommand(async _ => await ExecuteAddGlobal());
            DeleteGlobalCommand = new RelayCommand(p => ExecuteDeleteGlobal(p as Variable));
            DeleteTemporaryCommand = new RelayCommand(p => ExecuteDeleteTemporary(p as Variable));
        }

        private async Task ExecuteAddGlobal()
        {
            var result = await _dialogService.ShowAddVariableDialogAsync();

            if (result is (string name, string value) && !string.IsNullOrWhiteSpace(name))
            {
                _variableService.SetVariable(name, VariableScope.Global, value);
            }
        }

        private void ExecuteDeleteGlobal(Variable? variable)
        {
            if (variable != null)
            {
                _variableService.DeleteGlobalVariable(variable.Name);
            }
        }

        private void ExecuteDeleteTemporary(Variable? variable)
        {
            if (variable != null)
            {
                _variableService.DeleteTemporaryVariable(variable.Name);
            }
        }
    }
}