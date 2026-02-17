using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ActionCategoryAttribute : Attribute
    {
        public string Name { get; }
        public SymbolRegular Symbol { get; } = SymbolRegular.Folder24;
        public string? IconPath { get; set; }

        public ActionCategoryAttribute(string name)
        {
            Name = name;
        }

        public ActionCategoryAttribute(string name, SymbolRegular symbol)
        {
            Name = name;
            Symbol = symbol;
        }
    }
}