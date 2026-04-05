using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
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
                var allSources = new List<string>();
                var visited = new HashSet<string>();

                GetAllSourcesRecursive(obsService.Obs, sceneName, allSources, visited);

                return allSources.Distinct().OrderBy(name => name).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ObsSourceOptionsProvider] Помилка: {ex.Message}");
                return [$"Error: {ex.Message}"];
            }
        }

        private void GetAllSourcesRecursive(OBSWebsocket obs, string targetName, List<string> allSources, HashSet<string> visited)
        {
            if (!visited.Add(targetName)) return;

            List<SceneItemDetails> items = new List<SceneItemDetails>();

            try
            {
                items = obs.GetSceneItemList(targetName);
            }
            catch
            {
                try
                {
                    var request = new JObject { { "sceneName", targetName } };

                    var sendRequestMethod = obs.GetType().GetMethod(
                        "SendRequest",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        [typeof(string), typeof(JObject)],
                        null);

                    if (sendRequestMethod != null)
                    {
                        var response = sendRequestMethod.Invoke(obs, ["GetGroupSceneItemList", request]) as JObject;


                        if (response?["sceneItems"] is JArray sceneItemsArray)
                        {
                            var groupItems = sceneItemsArray.Select(m => (JObject)m).ToList();
                            items = groupItems.Select(g => new SceneItemDetails(g)).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ObsSourceOptionsProvider] Не вдалося прочитати групу {targetName}: {ex.Message}");
                    return;
                }
            }

            foreach (var item in items)
            {
                allSources.Add(item.SourceName);

                if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
                {
                    GetAllSourcesRecursive(obs, item.SourceName, allSources, visited);
                }
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
}