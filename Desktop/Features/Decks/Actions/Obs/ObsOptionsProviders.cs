using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    public class ObsSceneOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction action)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();

            if (!obsService.IsConnected)
                return ["OBS not connected"];

            try
            {
                var sceneList = obsService.Obs.GetSceneList();
                var options = sceneList.Scenes.Select(s => s.Name).ToList();

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

        private static string? GetSceneName(DeckAction action) => action switch
        {
            SourceVisibilityAction a => a.SceneName,
            MuteSourceAction a => a.SceneName,
            SwitchSceneAction a => a.SceneName,
            ScreenshotAction a => a.SceneName,
            _ => null
        };

        private static void SetSceneName(DeckAction action, string value)
        {
            if (action is SourceVisibilityAction v) v.SceneName = value;
            else if (action is MuteSourceAction m) m.SceneName = value;
            else if (action is SwitchSceneAction s) s.SceneName = value;
            else if (action is ScreenshotAction sc) sc.SceneName = value;
        }
    }

    public class ObsSourceOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction? action)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();

            if (!obsService.IsConnected)
                return ["OBS not connected"];

            var sceneNameProperty = action?.GetType().GetProperty("SceneName");
            var sceneName = sceneNameProperty?.GetValue(action) as string;

            if (string.IsNullOrWhiteSpace(sceneName))
                return ["Select a scene first"];

            try
            {
                var allSources = obsService.Obs.GetAllSourceNamesInScene(sceneName);

                return [.. allSources.Distinct().OrderBy(name => name)];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ObsSourceOptionsProvider] Помилка: {ex.Message}");
                return [$"Error: {ex.Message}"];
            }
        }
    }

    public class ObsMuteStateOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction? action)
        {
            return ["Toggle", "Mute", "Unmute"];
        }
    }

    public class ObsVisibilityStateOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction? action)
        {
            return ["Toggle", "Show", "Hide"];
        }
    }

    public class ObsStreamStateOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction? action)
        {
            return ["Toggle", "Start", "Stop"];
        }
    }

    public class ObsRecordStateOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction action)
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