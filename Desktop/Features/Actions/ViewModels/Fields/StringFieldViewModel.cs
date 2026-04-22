using System.Reflection;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class StringFieldViewModel(
        string label,
        string? hint,
        object targetAction, PropertyInfo property
    ) : ActionFieldViewModel(label, hint, targetAction, property)
    {
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
}