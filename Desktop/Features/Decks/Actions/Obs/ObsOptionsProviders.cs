using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                return sceneList.Scenes.Select(s => s.Name).ToList();
            }
            catch
            {
                return ["Error loading scenes"];
            }
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
}