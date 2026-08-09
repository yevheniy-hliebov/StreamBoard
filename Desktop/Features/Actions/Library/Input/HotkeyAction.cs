using StreamTabula.Core.Interop;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Exceptions;
using StreamTabula.Features.Actions.Models;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace StreamTabula.Features.Actions.Library.Input
{
    [ActionDiscriminator("hotkey")]
    public class HotkeyAction : InputBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Hotkey",
            DialogTitle: "Set Hotkey Combination",
            Icon: FluentIconType.Keyboard
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private bool _ctrl;

        [InputField("Ctrl", Hint = "Press Ctrl")]
        [JsonPropertyName("ctrl")]
        public bool Ctrl
        {
            get => _ctrl;
            set { _ctrl = value; OnPropertyChanged(); OnPropertyChanged(nameof(Label)); }
        }

        private bool _shift;

        [InputField("Shift", Hint = "Press Shift")]
        [JsonPropertyName("shift")]
        public bool Shift
        {
            get => _shift;
            set { _shift = value; OnPropertyChanged(); OnPropertyChanged(nameof(Label)); }
        }

        private bool _alt;

        [InputField("Alt", Hint = "Press Alt")]
        [JsonPropertyName("alt")]
        public bool Alt
        {
            get => _alt;
            set { _alt = value; OnPropertyChanged(); OnPropertyChanged(nameof(Label)); }
        }

        private bool _win;

        [InputField("Win", Hint = "Press Windows Key")]
        [JsonPropertyName("win")]
        public bool Win
        {
            get => _win;
            set { _win = value; OnPropertyChanged(); OnPropertyChanged(nameof(Label)); }
        }

        private string _keyToPress = "";

        [InputField("Main Key", Hint = "e.g., C, Enter, F5, Tab")]
        [JsonPropertyName("key_to_press")]
        public string KeyToPress
        {
            get => _keyToPress;
            set { _keyToPress = value; OnPropertyChanged(); OnPropertyChanged(nameof(Label)); }
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                var parts = new List<string>();
                if (Ctrl) parts.Add("Ctrl");
                if (Shift) parts.Add("Shift");
                if (Alt) parts.Add("Alt");
                if (Win) parts.Add("Win");
                if (!string.IsNullOrWhiteSpace(KeyToPress)) parts.Add(KeyToPress.ToUpper());

                return parts.Count > 0 ? string.Join(" + ", parts) : Metadata.Name;
            }
        }

        public override Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(KeyToPress) && !Ctrl && !Shift && !Alt && !Win)
                return Task.CompletedTask;

            try
            {
                var inputs = new List<Win32Input.INPUT>();

                if (Ctrl) inputs.Add(Win32Input.CreateInput(Win32Input.VK_CONTROL, isKeyUp: false));
                if (Shift) inputs.Add(Win32Input.CreateInput(Win32Input.VK_SHIFT, isKeyUp: false));
                if (Alt) inputs.Add(Win32Input.CreateInput(Win32Input.VK_MENU, isKeyUp: false));
                if (Win) inputs.Add(Win32Input.CreateInput(Win32Input.VK_LWIN, isKeyUp: false));

                if (!string.IsNullOrWhiteSpace(KeyToPress))
                {
                    ushort vk = GetVirtualKeyCode(KeyToPress);
                    if (vk != 0)
                    {
                        inputs.Add(Win32Input.CreateInput(vk, isKeyUp: false)); 
                        inputs.Add(Win32Input.CreateInput(vk, isKeyUp: true));
                    }
                }

                if (Win) inputs.Add(Win32Input.CreateInput(Win32Input.VK_LWIN, isKeyUp: true));
                if (Alt) inputs.Add(Win32Input.CreateInput(Win32Input.VK_MENU, isKeyUp: true));
                if (Shift) inputs.Add(Win32Input.CreateInput(Win32Input.VK_SHIFT, isKeyUp: true));
                if (Ctrl) inputs.Add(Win32Input.CreateInput(Win32Input.VK_CONTROL, isKeyUp: true));

                if (inputs.Count > 0)
                {
                    uint result = Win32Input.SendInput((uint)inputs.Count, inputs.ToArray(), Win32Input.INPUT.Size);

                    if (result == 0)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new HotkeyExecutionException($"SendInput failed with Win32 Error code: {errorCode}");
                    }
                }
            }
            catch (Exception ex) when (ex is not HotkeyExecutionException)
            {
                throw new HotkeyExecutionException($"Failed to simulate hotkey: {Label}", ex);
            }

            return Task.CompletedTask;
        }

        private static ushort GetVirtualKeyCode(string keyStr)
        {
            if (Enum.TryParse<Key>(keyStr, true, out Key wpfKey))
            {
                return (ushort)KeyInterop.VirtualKeyFromKey(wpfKey);
            }

            if (keyStr.Length == 1)
            {
                return (ushort)char.ToUpper(keyStr[0]);
            }

            return 0;
        }

        public override BaseAction Copy() => new HotkeyAction
        {
            Id = this.Id,
            Ctrl = this.Ctrl,
            Shift = this.Shift,
            Alt = this.Alt,
            Win = this.Win,
            KeyToPress = this.KeyToPress
        };
    }
}