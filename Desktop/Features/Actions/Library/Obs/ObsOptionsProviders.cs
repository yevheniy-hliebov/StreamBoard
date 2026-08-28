using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Integrations.OBS.Models;
using StreamTabula.Features.Integrations.OBS.Services;
using System.Diagnostics;

namespace StreamTabula.Features.Actions.Library.OBS;

public class OBSSceneOptionsProvider : IOptionsProvider
{
    public IEnumerable<object> GetOptions(BaseAction action)
    {
        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();

        if (!obs.IsConnected)
            return ["OBS not connected"];

        try
        {
            var sceneList = obs.GetSceneList();
            var options = sceneList.Scenes.Select(s => s.Name).ToList();
            options.Reverse();

            if (action is IHasSceneName sceneAction && string.IsNullOrEmpty(sceneAction.SceneName))
            {
                var first = options.FirstOrDefault();
                if (first != null) sceneAction.SceneName = first;
            }

            return options;
        }
        catch
        {
            return ["Error loading scenes"];
        }
    }
}

public class ObsSourceOptionsProvider : IOptionsProvider
{
    public IEnumerable<object> GetOptions(BaseAction? action)
    {
        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();

        if (!obs.IsConnected)
            return [new DropdownOption("OBS not connected")];

        var sceneNameProperty = action?.GetType().GetProperty("SceneName");
        var sceneName = sceneNameProperty?.GetValue(action) as string;

        if (string.IsNullOrWhiteSpace(sceneName))
            return [new DropdownOption("Select a scene first")];

        try
        {
            var sceneService = App.ServiceProvider.GetRequiredService<IOBSSceneService>();
            var allSources = sceneService.GetSourceList(sceneName);

            var options = allSources.Select(src =>
            {
                string displayOption = src.Name;

                if (src.Type == OBSSourceType.Group)
                    displayOption = $"📁 {src.Name}";
                else if (src.Type == OBSSourceType.Group)
                    displayOption = src.IsPartOfGroup ? $"   └─ 🎬 {src.Name}" : $"🎬 {src.Name}";
                else if (src.IsPartOfGroup)
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