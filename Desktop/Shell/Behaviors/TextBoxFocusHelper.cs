using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StreamTabula.Shell.Behaviors;

public static class TextBoxFocusHelper
{
    public static void Attach(Window window)
    {
        window.PreviewMouseDown += (s, e) =>
        {
            if (Keyboard.FocusedElement is System.Windows.Controls.Primitives.TextBoxBase focusedTextBox)
            {
                DependencyObject? clickedElement = e.OriginalSource as DependencyObject;
                bool clickedInsideAnyTextBox = false;
                var current = clickedElement;

                while (current != null)
                {
                    if (current is System.Windows.Controls.Primitives.TextBoxBase)
                    {
                        clickedInsideAnyTextBox = true;
                        break;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }

                if (!clickedInsideAnyTextBox)
                {
                    Keyboard.ClearFocus();
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(focusedTextBox), null);
                    window.Focus();
                }
            }
        };
    }
}