using System.Reflection;

namespace StreamTabula.Features.Actions.ViewModels
{
    public class BoolFieldViewModel(
        string label,
        string? hint,
        object targetAction,
        PropertyInfo property
    ) : ActionFieldViewModel(label, hint, targetAction, property)
    {
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
}