using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public abstract class ActionSettingViewModel : ObservableObject
    {
        public string Label { get; }
        public string? Hint { get; }
        protected object TargetAction { get; }
        protected PropertyInfo Property { get; }

        protected ActionSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
        {
            Label = label;
            Hint = hint;
            TargetAction = targetAction;
            Property = property;
        }
    }

    public class StringSettingViewModel : ActionSettingViewModel
    {
        public StringSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class IntSettingViewModel : ActionSettingViewModel
    {
        public IntSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public int Value
        {
            get => Property.GetValue(TargetAction) is int val ? val : 0;
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class BoolSettingViewModel : ActionSettingViewModel
    {
        public BoolSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public bool Value
        {
            get => Property.GetValue(TargetAction) is bool val && val;
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class DropdownSettingViewModel : ActionSettingViewModel
    {
        private readonly IOptionsProvider _provider;
        private readonly DeckAction _action;

        public ObservableCollection<string> Options { get; } = new();

        public DropdownSettingViewModel(
            string label,
            string? hint,
            DeckAction targetAction,
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