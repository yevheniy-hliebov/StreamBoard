using StreamTabula.Features.Decks.Models;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Features.Decks.Views.Components.Canvas;

public partial class DeckButtonView : UserControl
{
    public DeckButtonView()
    {
        InitializeComponent();
    }

    public int ButtonIndex
    {
        get { return (int)GetValue(ButtonIndexProperty); }
        set { SetValue(ButtonIndexProperty, value); }
    }

    public static readonly DependencyProperty ButtonIndexProperty =
        DependencyProperty.Register(nameof(ButtonIndex), typeof(int), typeof(DeckButtonView), new PropertyMetadata(0));

    public DeckButtonConfig ButtonConfig
    {
        get { return (DeckButtonConfig)GetValue(ButtonConfigProperty); }
        set { SetValue(ButtonConfigProperty, value); }
    }

    public static readonly DependencyProperty ButtonConfigProperty =
        DependencyProperty.Register(nameof(ButtonConfig), typeof(DeckButtonConfig), typeof(DeckButtonView));
}
