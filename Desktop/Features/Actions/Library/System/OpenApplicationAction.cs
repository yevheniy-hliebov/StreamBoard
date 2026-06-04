using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;

namespace StreamTabula.Features.Actions.Library.System
{
    [ActionDiscriminator("open_application")]
    public class OpenApplicationAction : SystemBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open Application",
            DialogTitle: "Enter Application Path",
            Icon: FluentIconType.Apps
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _appPath = "";

        [PathField("Application Path", PathSelectionType.File, Filter = "Executables (*.exe)|*.exe|Shortcuts (*.lnk)|*.lnk|All files (*.*)|*.*", Hint = "Select .exe or shortcut...")]
        [JsonPropertyName("app_path")]
        public string AppPath
        {
            get => _appPath;
            set
            {
                _appPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        private string _arguments = "";

        [InputField("Arguments", Hint = "Enter startup arguments (optional)...")]
        [JsonPropertyName("arguments")]
        public string Arguments
        {
            get => _arguments;
            set
            {
                _arguments = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(AppPath)
            ? Metadata.Name : $"{Metadata.Name} ({Path.GetFileNameWithoutExtension(AppPath)})";

        public override Task ExecuteAsync(ActionExecutionContext context)
        {
            context.RuntimeVariables["openAppSuccess"] = "false";
            context.RuntimeVariables["openAppError"] = "";
            context.RuntimeVariables["openAppPID"] = "";

            if (string.IsNullOrWhiteSpace(AppPath)) return Task.CompletedTask;

            try
            {
                string resolvedPath = ResolveVariable(AppPath, context);
                string resolvedArguments = ResolveVariable(Arguments, context);

                if (string.IsNullOrWhiteSpace(resolvedPath)) return Task.CompletedTask;

                var psi = new ProcessStartInfo
                {
                    FileName = resolvedPath,
                    Arguments = resolvedArguments,
                    UseShellExecute = true
                };

                var process = Process.Start(psi);

                context.RuntimeVariables["openAppSuccess"] = "true";

                if (process != null)
                {
                    try
                    {
                        context.RuntimeVariables["openAppPID"] = process.Id.ToString();
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                context.RuntimeVariables["openAppError"] = ex.Message;
            }

            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new OpenApplicationAction
        {
            Id = this.Id,
            AppPath = this.AppPath,
            Arguments = this.Arguments
        };
    }
}