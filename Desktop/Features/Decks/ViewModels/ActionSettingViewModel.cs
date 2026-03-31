using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using System.Collections.ObjectModel;
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
        public ObservableCollection<string> Options { get; } = new();

        public DropdownSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property, IOptionsProvider provider)
            : base(label, hint, targetAction, property)
        {
            var options = provider.GetOptions();
            foreach (var opt in options) Options.Add(opt);
        }

        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }

        public ICommand SelectOptionCommand => new RelayCommand<string>(option =>
        {
            if (option != null)
            {
                Value = option;
            }
        });
    }
}