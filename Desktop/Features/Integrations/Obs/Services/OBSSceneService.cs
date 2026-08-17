using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using StreamTabula.Features.Integrations.Obs.Models;
using System.Reflection;

namespace StreamTabula.Features.Integrations.Obs.Services;

public interface IOBSSceneService
{
    List<OBSSourceDetails> GetSourceList(string sceneName);
    OBSSourceDetails? GetSourceInfo(string sceneName, string sourceName);
}

public class OBSSceneService(IOBSWebsocket OBS) : IOBSSceneService
{
    public List<OBSSourceDetails> GetSourceList(string sceneName)
    {
        var sources = new List<OBSSourceDetails>();

        var items = OBS.GetSceneItemList(sceneName);
        items.Reverse();

        foreach (var item in items)
        {
            OBSSourceType itemType = OBSSourceType.NormalSource;

            List<SceneItemDetails>? groupItems = null;

            if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
            {
                groupItems = GetGroupSourceList(item.SourceName);
            }

            var isItemGroup = groupItems != null;

            if (isItemGroup)
            {
                itemType = OBSSourceType.Group;
            } else if (item.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
            {
                itemType = OBSSourceType.NestedScene;
            }

            sources.Add(new OBSSourceDetails
            {
                Id = item.ItemId,
                Name = item.SourceName,
                Type = itemType,
                ParentName = sceneName,
                SceneName = sceneName
            });

            if (isItemGroup)
            {
                groupItems!.Reverse();
                foreach (var groupItem in groupItems!)
                {
                    OBSSourceType groupItemType = OBSSourceType.NormalSource;
                    if (groupItem.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
                    {
                        groupItemType = OBSSourceType.NestedScene;
                    }

                    sources.Add(new OBSSourceDetails
                    {
                        Id = groupItem.ItemId,
                        Name = groupItem.SourceName,
                        Type = groupItemType,
                        ParentName = item.SourceName,
                        SceneName = sceneName
                    });
                }
            }
        }        

        return sources;
    }

    public OBSSourceDetails? GetSourceInfo(string sceneName, string sourceName)
    {
        var items = OBS.GetSceneItemList(sceneName);

        var target = items.FirstOrDefault(i => i.SourceName == sourceName);
        if (target != null)
        {
            OBSSourceType type = OBSSourceType.NormalSource;

            if (target.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE)
            {
                type = GetGroupSourceList(target.SourceName) != null
                    ? OBSSourceType.Group
                    : OBSSourceType.NestedScene;
            }

            return new OBSSourceDetails
            {
                Id = target.ItemId,
                Name = target.SourceName,
                Type = type,
                ParentName = sceneName,
                SceneName = sceneName
            };
        }

        foreach (var item in items.Where(i => i.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE))
        {
            var groupItems = GetGroupSourceList(item.SourceName);
            if (groupItems != null)
            {
                var targetInGroup = groupItems.FirstOrDefault(i => i.SourceName == sourceName);
                if (targetInGroup != null)
                {
                    OBSSourceType type = targetInGroup.SourceType == SceneItemSourceType.OBS_SOURCE_TYPE_SCENE
                        ? OBSSourceType.NestedScene
                        : OBSSourceType.NormalSource;

                    return new OBSSourceDetails
                    {
                        Id = targetInGroup.ItemId,
                        Name = targetInGroup.SourceName,
                        Type = type,
                        ParentName = item.SourceName,
                        SceneName = sceneName
                    };
                }
            }
        }

        return null;
    }

    private List<SceneItemDetails>? GetGroupSourceList (string groupName)
    {
        try
        {
            var request = new JObject { { "sceneName", groupName } };
            var sendRequestMethod = OBS.GetType().GetMethod(
                "SendRequest",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                [typeof(string), typeof(JObject)],
                null);

            if (sendRequestMethod != null)
            {
                var response = sendRequestMethod.Invoke(OBS, ["GetGroupSceneItemList", request]) as JObject;

                if (response?["sceneItems"] is JArray sceneItemsArray)
                {
                    return sceneItemsArray
                        .Select(m => new SceneItemDetails((JObject)m))
                        .ToList();
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}
