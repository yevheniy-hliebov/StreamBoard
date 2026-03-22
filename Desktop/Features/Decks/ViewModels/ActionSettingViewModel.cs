using StreamBoard.Core;
using System.Reflection;

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
}