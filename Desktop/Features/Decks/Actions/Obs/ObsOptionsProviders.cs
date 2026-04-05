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
}