using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Windows.Input;
using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Helpers;

namespace StreamBoard.Features.Actions.Library.Input
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
                var inputs = new List<Win32InputHelper.INPUT>();

                if (Ctrl) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_CONTROL, false));
                if (Shift) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_SHIFT, false));
                if (Alt) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_MENU, false));
                if (Win) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_LWIN, false));

                if (!string.IsNullOrWhiteSpace(KeyToPress))
                {
                    ushort vk = GetVirtualKeyCode(KeyToPress);
                    if (vk != 0)
                    {
                        inputs.Add(Win32InputHelper.CreateInput(vk, false)); // Key Down
                        inputs.Add(Win32InputHelper.CreateInput(vk, true));  // Key Up
                    }
                }

                if (Win) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_LWIN, true));
                if (Alt) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_MENU, true));
                if (Shift) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_SHIFT, true));
                if (Ctrl) inputs.Add(Win32InputHelper.CreateInput(Win32InputHelper.VK_CONTROL, true));

                if (inputs.Count > 0)
                {
                    uint result = Win32InputHelper.SendInput((uint)inputs.Count, inputs.ToArray(), Win32InputHelper.INPUT.Size);

                    if (result == 0)
                    {
                        int error = Marshal.GetLastWin32Error();
                        Debug.WriteLine($"SendInput failed with Win32 Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not simulate hotkey: {ex.Message}");
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