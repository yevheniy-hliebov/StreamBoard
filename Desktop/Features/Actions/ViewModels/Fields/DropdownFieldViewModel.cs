using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using StreamTabula.Features.Actions.Models;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Actions.ViewModels
{
    public class DropdownFieldViewModel : ActionFieldViewModel
    {
        private readonly IOptionsProvider _provider;
        private readonly BaseAction _action;

        public ObservableCollection<string> Options { get; } = new();

        public DropdownFieldViewModel(
            string label,
            string? hint,
            BaseAction targetAction,
            PropertyInfo property,
            IOptionsProvider provider)
            : base(label, hint, targetAction, property)
        {
            _action = targetAction;
            _provider = provider;

            _action.PropertyChanged += OnActionPropertyChanged;

            RefreshOptions();
        }

        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                if (Value != value)
                {
                    Property.SetValue(TargetAction, value);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectOptionCommand => new RelayCommand<string>(option =>
        {
            if (option != null)
            {
                Value = option;
            }
        });

        private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshOptions();
        }

        private void RefreshOptions()
        {
            var newOptions = _provider.GetOptions(_action) ?? [];

            if (!Options.SequenceEqual(newOptions))
            {
                Options.Clear();
                foreach (var opt in newOptions)
                {
                    Options.Add(opt);
                }

                OnPropertyChanged(nameof(Options));
            }
        }
    }
}