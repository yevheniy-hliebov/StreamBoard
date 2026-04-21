using System.Reflection;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class IntFieldViewModel(
        string label,
        string? hint,
        object targetAction,
        PropertyInfo property
    ) : ActionFieldViewModel(label, hint, targetAction, property)
    {
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