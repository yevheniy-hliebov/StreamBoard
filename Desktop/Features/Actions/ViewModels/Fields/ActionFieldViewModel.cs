using System.Reflection;
using StreamBoard.Core;

namespace StreamBoard.Features.Actions.ViewModels
{
    public abstract class ActionFieldViewModel(
        string label,
        string? hint,
        object targetAction,
        PropertyInfo property
    ) : ObservableObject
    {
        public string Label { get; } = label;
        public string? Hint { get; } = hint;
        protected object TargetAction { get; } = targetAction;
        protected PropertyInfo Property { get; } = property;
    }
}