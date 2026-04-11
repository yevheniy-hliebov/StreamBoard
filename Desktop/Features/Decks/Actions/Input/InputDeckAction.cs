using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.Input
{
    [ActionDiscriminator("text_string")]
    public class TextStringAction : InputDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Text String",
            DialogTitle: "Enter Text to Type",
            Icon: FluentIconType.Rename
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _textToType = "";

        [ActionSetting("Text", "Enter text to type...")]
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

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(TextToType)
            ? Metadata.Name
            : $"{Metadata.Name} (\"{TextToType}\")";

        // --- Win32 API Константи та Структури ---
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
                // Створюємо масив дій: для кожного символу потрібне натискання (Down) та відпускання (Up)
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

        public override DeckAction Copy() => new TextStringAction
        {
            Id = this.Id,
            TextToType = this.TextToType
        };
    }
}