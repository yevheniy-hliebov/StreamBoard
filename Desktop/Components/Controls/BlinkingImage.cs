using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StreamTabula.Components.Controls;

public class BlinkingImage : Image
{

    public bool IsBlinking
    {
        get => (bool)GetValue(IsBlinkingProperty);
        set => SetValue(IsBlinkingProperty, value);
    }

    public static readonly DependencyProperty IsBlinkingProperty =
        DependencyProperty.Register(
            nameof(IsBlinking),
            typeof(bool),
            typeof(BlinkingImage),
            new PropertyMetadata(false, OnIsBlinkingChanged));
    
    public TimeSpan BlinkDuration
    {
        get => (TimeSpan)GetValue(BlinkDurationProperty);
        set => SetValue(BlinkDurationProperty, value);
    }

    public static readonly DependencyProperty BlinkDurationProperty =
        DependencyProperty.Register(
            nameof(BlinkDuration),
            typeof(TimeSpan),
            typeof(BlinkingImage),
            new PropertyMetadata(TimeSpan.FromSeconds(0.5), OnBlinkDurationChanged));

    private static void OnIsBlinkingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlinkingImage img)
        {
            if ((bool)e.NewValue)
            {
                img.StartBlinking();
            }
            else
            {
                img.StopBlinking();
            }
        }
    }

    private static void OnBlinkDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlinkingImage img && img.IsBlinking)
        {
            img.StartBlinking();
        }
    }

    private void StartBlinking()
    {
        var halfDuration = TimeSpan.FromMilliseconds(BlinkDuration.TotalMilliseconds / 2);

        var blinkAnimation = new DoubleAnimationUsingKeyFrames
        {
            Duration = new Duration(BlinkDuration),
            RepeatBehavior = RepeatBehavior.Forever
        };

        blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.Zero)));

        blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(halfDuration)));

        BeginAnimation(OpacityProperty, blinkAnimation);
    }

    private void StopBlinking()
    {
        BeginAnimation(OpacityProperty, null);
        Opacity = 1.0;
    }
}
