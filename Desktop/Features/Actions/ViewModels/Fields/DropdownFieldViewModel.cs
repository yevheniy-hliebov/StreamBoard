using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using StreamTabula.Components.Controls;
using StreamTabula.Features.Actions.Models;

namespace StreamTabula.Features.Actions.ViewModels
{
    public class DropdownFieldViewModel : ActionFieldViewModel
    {
        private readonly IOptionsProvider _provider;
        private readonly BaseAction _action;

        public ObservableCollection<DropdownOption> Options { get; } = new();

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

        public object? Value
        {
            get => Property.GetValue(TargetAction);
            set
            {
                if (!Equals(Value, value))
                {
                    Property.SetValue(TargetAction, value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedOption)); // Оновлюємо вибраний об'єкт
                }
            }
        }

        // Замість команди та SelectedDisplayOption робимо одну повноцінну властивість
        public DropdownOption? SelectedOption
        {
            get
            {
                return Options.FirstOrDefault(o => Equals(o.Value, Value));
            }
            set
            {
                if (value != null && !Equals(Value, value.Value))
                {
                    Value = value.Value; // Тут виконується та сама логіка, що була в команді
                }
            }
        }

        private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshOptions();
        }

        private void RefreshOptions()
        {
            var rawOptions = _provider.GetOptions(_action) ?? [];

            Options.Clear();

            foreach (var opt in rawOptions)
            {
                if (opt is DropdownOption dropOption)
                {
                    Options.Add(dropOption);
                }
                else if (opt is string strOption)
                {
                    Options.Add(new DropdownOption(strOption));
                }
                else if (opt != null)
                {
                    Options.Add(new DropdownOption(opt));
                }
            }

            OnPropertyChanged(nameof(Options));
            OnPropertyChanged(nameof(SelectedOption));
        }
    }
}