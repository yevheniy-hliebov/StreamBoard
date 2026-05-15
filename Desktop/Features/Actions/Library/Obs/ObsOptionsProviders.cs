using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Integrations.Obs.Models;
using StreamTabula.Features.Integrations.Obs.Services;

namespace StreamTabula.Features.Actions.Library.Obs
{
    public class ObsSceneOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction action)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();

            if (!obsService.IsConnected)
                return ["OBS not connected"];

            try
            {
                var sceneList = obsService.Obs.GetSceneList();
                var options = sceneList.Scenes.Select(s => s.Name).ToList();
                options.Reverse();

                if (string.IsNullOrEmpty(GetSceneName(action)))
                {
                    var first = options.FirstOrDefault();
                    if (first != null) SetSceneName(action, first);
                }

                return options;
            }
            catch
            {
                return ["Error loading scenes"];
            }
        }

        private static string? GetSceneName(BaseAction action) => action switch
        {
            SourceVisibilityAction a => a.SceneName,
            MuteSourceAction a => a.SceneName,
            SwitchSceneAction a => a.SceneName,
            ScreenshotAction a => a.SceneName,
            _ => null
        };

        private static void SetSceneName(BaseAction action, string value)
        {
            if (action is SourceVisibilityAction v) v.SceneName = value;
            else if (action is MuteSourceAction m) m.SceneName = value;
            else if (action is SwitchSceneAction s) s.SceneName = value;
            else if (action is ScreenshotAction sc) sc.SceneName = value;
        }
    }

    public class ObsSourceOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction? action)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();

            if (!obsService.IsConnected)
                return [new DropdownOption("OBS not connected")];

            var sceneNameProperty = action?.GetType().GetProperty("SceneName");
            var sceneName = sceneNameProperty?.GetValue(action) as string;

            if (string.IsNullOrWhiteSpace(sceneName))
                return [new DropdownOption("Select a scene first")];

            try
            {
                List<ObsSourceItem> allSources = obsService.Obs.GetAllSourcesDetailsInScene(sceneName);

                var options = allSources.Select(src =>
                {
                    string displayOption = src.Name;

                    if (src.IsGroup)
                        displayOption = $"📁 {src.Name}";
                    else if (src.IsNestedScene)
                        displayOption = src.IsInGroup ? $"   └─ 🎬 {src.Name}" : $"🎬 {src.Name}";
                    else if (src.IsInGroup)
                        displayOption = $"   └─ {src.Name}";

                    string displaySelectedOption = src.Name;

                    return new DropdownOption(
                        value: src.Name,
                        displayOption: displayOption,
                        displaySelectedOption: displaySelectedOption
                    );
                }).ToList();

                return options;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ObsSourceOptionsProvider] Помилка: {ex.Message}");
                return [new DropdownOption($"Error: {ex.Message}")];
            }
        }
    }

    public class ObsMuteStateOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction? action)
        {
            return ["Toggle", "Mute", "Unmute"];
        }
    }

    public class ObsVisibilityStateOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction? action)
        {
            return ["Toggle", "Show", "Hide"];
        }
    }

    public class ObsOutputStateOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction? action)
        {
            return ["Toggle", "Start", "Stop"];
        }
    }

    public class ObsRecordStateOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction action)
        {
            return [
                "Toggle",
                "Start",
                "Stop",
                "Pause",
                "Resume",
                "Toggle Pause"
            ];
        }
    }
}