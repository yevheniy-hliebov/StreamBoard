using StreamTabula.Features.Decks.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.Views.Components.Canvas;

public partial class GridCanvasView : UserControl
{
    public GridCanvasView() => InitializeComponent();

    public ObservableCollection<DeckButtonSlot> Buttons
    {
        get => (ObservableCollection<DeckButtonSlot>)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }
    public static readonly DependencyProperty ButtonsProperty =
        DependencyProperty.Register(nameof(Buttons), typeof(ObservableCollection<DeckButtonSlot>), typeof(GridCanvasView), new PropertyMetadata(null));

    public GridCanvasConfig CanvasConfig
    {
        get => (GridCanvasConfig)GetValue(CanvasConfigProperty);
        set => SetValue(CanvasConfigProperty, value);
    }
    public static readonly DependencyProperty CanvasConfigProperty =
        DependencyProperty.Register(nameof(CanvasConfig), typeof(GridCanvasConfig), typeof(GridCanvasView), new PropertyMetadata(null));

    public bool IsClickMode
    {
        get => (bool)GetValue(IsClickModeProperty);
        set => SetValue(IsClickModeProperty, value);
    }
    public static readonly DependencyProperty IsClickModeProperty =
        DependencyProperty.Register(nameof(IsClickMode), typeof(bool), typeof(GridCanvasView), new PropertyMetadata(false));

    public bool IsEditorMode
    {
        get => (bool)GetValue(IsEditorModeProperty);
        set => SetValue(IsEditorModeProperty, value);
    }
    public static readonly DependencyProperty IsEditorModeProperty =
        DependencyProperty.Register(nameof(IsEditorMode), typeof(bool), typeof(GridCanvasView), new PropertyMetadata(false));

    public ICommand ClickButtonCommand
    {
        get => (ICommand)GetValue(ClickButtonCommandProperty);
        set => SetValue(ClickButtonCommandProperty, value);
    }
    public static readonly DependencyProperty ClickButtonCommandProperty =
        DependencyProperty.Register(nameof(ClickButtonCommand), typeof(ICommand), typeof(GridCanvasView), new PropertyMetadata(null));

    public ICommand CopyCommand
    {
        get => (ICommand)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }
    public static readonly DependencyProperty CopyCommandProperty =
        DependencyProperty.Register(nameof(CopyCommand), typeof(ICommand), typeof(GridCanvasView), new PropertyMetadata(null));

    public ICommand PasteCommand
    {
        get => (ICommand)GetValue(PasteCommandProperty);
        set => SetValue(PasteCommandProperty, value);
    }
    public static readonly DependencyProperty PasteCommandProperty =
        DependencyProperty.Register(nameof(PasteCommand), typeof(ICommand), typeof(GridCanvasView), new PropertyMetadata(null));

    public ICommand CutCommand
    {
        get => (ICommand)GetValue(CutCommandProperty);
        set => SetValue(CutCommandProperty, value);
    }
    public static readonly DependencyProperty CutCommandProperty =
        DependencyProperty.Register(nameof(CutCommand), typeof(ICommand), typeof(GridCanvasView), new PropertyMetadata(null));

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(GridCanvasView), new PropertyMetadata(null));
}