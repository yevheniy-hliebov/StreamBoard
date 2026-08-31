using GongSolutions.Wpf.DragDrop;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.Views.Components.PageList;

public partial class DeckPageListView : UserControl
{
    public DeckPageListView() => InitializeComponent();

    public IEnumerable AllPages
    {
        get => (IEnumerable)GetValue(AllPagesProperty);
        set => SetValue(AllPagesProperty, value);
    }
    public static readonly DependencyProperty AllPagesProperty =
        DependencyProperty.Register(nameof(AllPages), typeof(IEnumerable), typeof(DeckPageListView), new PropertyMetadata(null));

    public object SelectedPage
    {
        get => GetValue(SelectedPageProperty);
        set => SetValue(SelectedPageProperty, value);
    }
    public static readonly DependencyProperty SelectedPageProperty =
        DependencyProperty.Register(nameof(SelectedPage), typeof(object), typeof(DeckPageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsEditorMode
    {
        get => (bool)GetValue(IsEditorModeProperty);
        set => SetValue(IsEditorModeProperty, value);
    }
    public static readonly DependencyProperty IsEditorModeProperty =
        DependencyProperty.Register(nameof(IsEditorMode), typeof(bool), typeof(DeckPageListView), new PropertyMetadata(false));

    public bool IsRenameMode
    {
        get => (bool)GetValue(IsRenameModeProperty);
        set => SetValue(IsRenameModeProperty, value);
    }
    public static readonly DependencyProperty IsRenameModeProperty =
        DependencyProperty.Register(nameof(IsRenameMode), typeof(bool), typeof(DeckPageListView), new PropertyMetadata(false));

    public IDropTarget DropHandler
    {
        get => (IDropTarget)GetValue(DropHandlerProperty);
        set => SetValue(DropHandlerProperty, value);
    }
    public static readonly DependencyProperty DropHandlerProperty =
        DependencyProperty.Register(nameof(DropHandler), typeof(IDropTarget), typeof(DeckPageListView), new PropertyMetadata(null));
    public ICommand AddPageCommand
    {
        get => (ICommand)GetValue(AddPageCommandProperty);
        set => SetValue(AddPageCommandProperty, value);
    }
    public static readonly DependencyProperty AddPageCommandProperty = DependencyProperty.Register(nameof(AddPageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand CopyPageCommand
    {
        get => (ICommand)GetValue(CopyPageCommandProperty);
        set => SetValue(CopyPageCommandProperty, value);
    }
    public static readonly DependencyProperty CopyPageCommandProperty = DependencyProperty.Register(nameof(CopyPageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand PastePageCommand
    {
        get => (ICommand)GetValue(PastePageCommandProperty);
        set => SetValue(PastePageCommandProperty, value);
    }
    public static readonly DependencyProperty PastePageCommandProperty = DependencyProperty.Register(nameof(PastePageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand CutPageCommand
    {
        get => (ICommand)GetValue(CutPageCommandProperty);
        set => SetValue(CutPageCommandProperty, value);
    }
    public static readonly DependencyProperty CutPageCommandProperty = DependencyProperty.Register(nameof(CutPageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand DuplicatePageCommand
    {
        get => (ICommand)GetValue(DuplicatePageCommandProperty);
        set => SetValue(DuplicatePageCommandProperty, value);
    }
    public static readonly DependencyProperty DuplicatePageCommandProperty = DependencyProperty.Register(nameof(DuplicatePageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand RenamePageCommand
    {
        get => (ICommand)GetValue(RenamePageCommandProperty);
        set => SetValue(RenamePageCommandProperty, value);
    }
    public static readonly DependencyProperty RenamePageCommandProperty = DependencyProperty.Register(nameof(RenamePageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand DeletePageCommand
    {
        get => (ICommand)GetValue(DeletePageCommandProperty);
        set => SetValue(DeletePageCommandProperty, value);
    }
    public static readonly DependencyProperty DeletePageCommandProperty = DependencyProperty.Register(nameof(DeletePageCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));

    public ICommand EndRenameCommand
    {
        get => (ICommand)GetValue(EndRenameCommandProperty);
        set => SetValue(EndRenameCommandProperty, value);
    }
    public static readonly DependencyProperty EndRenameCommandProperty = DependencyProperty.Register(nameof(EndRenameCommand), typeof(ICommand), typeof(DeckPageListView), new PropertyMetadata(null));
}