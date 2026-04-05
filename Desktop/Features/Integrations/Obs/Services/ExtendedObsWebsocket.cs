using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace StreamBoard.Features.Integrations.Obs.Services
{
    public class ExtendedObsWebsocket : OBSWebsocket
    {
        public List<string> GetAllSourceNamesInScene(string sceneName)
        {
            var allSources = new List<string>();
            var visited = new HashSet<string>();
            GetAllSourcesRecursiveHelper(sceneName, allSources, visited);
            return allSources;
        }

        public (string ParentScene, int SourceId)? GetSourceParentAndId(string rootScene, string targetSourceName)
        {
            var visited = new HashSet<string>();
            return GetSourceParentAndIdHelper(rootScene, targetSourceName, visited);
        }

        private void GetAllSourcesRecursiveHelper(string targetName, List<string> allSources, HashSet<string> visited)
        {
            if (!visited.Add(targetName)) return;

            var items = TryGetSceneItems(targetName);

            foreach (var item in items)
            {
                allSources.Add(item.SourceName);

                if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
                {
                    GetAllSourcesRecursiveHelper(item.SourceName, allSources, visited);
                }
            }
        }

        private (string ParentScene, int SourceId)? GetSourceParentAndIdHelper(string currentScene, string targetSourceName, HashSet<string> visited)
        {
            if (!visited.Add(currentScene)) return null;

            var items = TryGetSceneItems(currentScene);

            foreach (var item in items)
            {
                if (item.SourceName == targetSourceName)
                {
                    return (currentScene, item.ItemId);
                }

                if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
                {
                    var found = GetSourceParentAndIdHelper(item.SourceName, targetSourceName, visited);
                    if (found != null) return found;
                }
            }

            return null;
        }

        private List<SceneItemDetails> TryGetSceneItems(string targetName)
        {
            List<SceneItemDetails> items = [];

            try
            {
                items = GetSceneItemList(targetName);
            }
            catch
            {
                try
                {
                    var request = new JObject { { "sceneName", targetName } };

                    var sendRequestMethod = GetType().GetMethod(
                        "SendRequest",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        [typeof(string), typeof(JObject)],
                        null);

                    if (sendRequestMethod != null)
                    {
                        var response = sendRequestMethod.Invoke(this, ["GetGroupSceneItemList", request]) as JObject;

                        if (response?["sceneItems"] is JArray sceneItemsArray)
                        {
                            var groupItems = sceneItemsArray.Select(m => (JObject)m).ToList();
                            items = groupItems.Select(g => new SceneItemDetails(g)).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ExtendedObsWebsocket] Не вдалося прочитати групу {targetName}: {ex.Message}");
                }
            }

            return items;
        }
    }
}