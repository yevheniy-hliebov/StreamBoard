using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Input;

[ActionDiscriminator("text_string")]
[ActionInfo("Text String", "Enter Text to Type", FluentIconType.Rename)]
public class TextStringAction : InputBaseAction
{
    private string _textToType = "";

    [InputField("Text", Hint = "Enter text to type...")]
    [JsonPropertyName("text_to_type")]
    public string TextToType
    {
        get => _textToType;
        set
        {
            _textToType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Label));
        }
    }

    private const int INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_UNICODE = 0x0004;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public INPUTUNION U;
        public static int Size => Marshal.SizeOf(typeof(INPUT));
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct INPUTUNION
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT { public int dx, dy; public uint mouseData, dwFlags, time; public IntPtr dwExtraInfo; }

    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT { public uint uMsg; public ushort wParamL, wParamH; }

    public override Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrEmpty(TextToType)) return Task.CompletedTask;

        try
        {
            INPUT[] inputs = new INPUT[TextToType.Length * 2];

            for (int i = 0; i < TextToType.Length; i++)
            {
                char c = TextToType[i];

                // Key Down
                inputs[i * 2] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new INPUTUNION
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = c,
                            dwFlags = KEYEVENTF_UNICODE,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                // Key Up
                inputs[i * 2 + 1] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new INPUTUNION
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = c,
                            dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };
            }

            SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not type text: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}