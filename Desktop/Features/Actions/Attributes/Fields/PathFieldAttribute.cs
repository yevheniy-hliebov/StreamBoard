using StreamBoard.Features.Actions.Models;

namespace StreamBoard.Features.Actions.Attributes
{
    public class PathFieldAttribute(
        string label,
        PathSelectionType selectionType = PathSelectionType.File
    ) : ActionFieldAttribute(label)
    {
        public PathSelectionType SelectionType { get; } = selectionType;

        public string Filter { get; set; } = "All files (*.*)|*.*";
    }
}