using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using StreamTabula.Features.Integrations.Obs.Models;
using System.Diagnostics;
using System.Reflection;

namespace StreamTabula.Features.Integrations.Obs.Services
{
    public class ExtendedObsWebsocket : OBSWebsocket
    {
        public List<ObsSourceItem> GetAllSourcesDetailsInScene(string sceneName)
        {
            var allSources = new List<ObsSourceItem>();
            var visited = new HashSet<string>();
            GetAllSourcesRecursiveHelper(sceneName, allSources, visited, null);
            return allSources;
        }

        private void GetAllSourcesRecursiveHelper(string targetName, List<ObsSourceItem> allSources, HashSet<string> visited, string? parentGroupName)
        {
            if (!visited.Add(targetName)) return;

            var items = TryGetSceneItems(targetName, out bool _);
            items.Reverse();

            foreach (var item in items)
            {
                bool isItemGroup = IsGroup(item);
                bool isNestedScene = !isItemGroup && item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE;

                allSources.Add(new ObsSourceItem
                {
                    Name = item.SourceName,
                    IsGroup = isItemGroup,
                    IsNestedScene = isNestedScene,
                    IsInGroup = !string.IsNullOrEmpty(parentGroupName),
                    ParentGroupName = parentGroupName
                });

                if (isItemGroup)
                {
                    GetAllSourcesRecursiveHelper(item.SourceName, allSources, visited, item.SourceName);
                }
            }
        }

        private bool IsGroup(SceneItemDetails item)
        {
            var prop = item.GetType().GetProperty("IsGroup");
            if (prop != null && prop.GetValue(item) is bool isGroup)
                return isGroup;

            if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
            {
                TryGetSceneItems(item.SourceName, out bool isG);
                return isG;
            }
            return false;
        }

        private List<SceneItemDetails> TryGetSceneItems(string targetName, out bool isGroup)
        {
            List<SceneItemDetails> items = [];
            isGroup = false;

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
                            isGroup = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ExtendedObsWebsocket] Failed to read group {targetName}: {ex.Message}");
                }
            }

            return items;
        }

        public (string ParentName, int SourceId)? GetSourceParentAndId(string rootScene, string targetSourceName)
        {
            var visited = new HashSet<string>();
            return GetSourceParentAndIdHelper(rootScene, targetSourceName, visited);
        }

        private (string ParentName, int SourceId)? GetSourceParentAndIdHelper(string currentContainer, string targetSourceName, HashSet<string> visited)
        {
            if (!visited.Add(currentContainer)) return null;

            var items = TryGetSceneItems(currentContainer, out bool _);

            foreach (var item in items)
            {
                if (item.SourceName == targetSourceName)
                {
                    return (currentContainer, item.ItemId);
                }

                if (IsGroup(item))
                {
                    var found = GetSourceParentAndIdHelper(item.SourceName, targetSourceName, visited);
                    if (found != null) return found;
                }
            }

            return null;
        }
    }
}