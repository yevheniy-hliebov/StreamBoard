using StreamBoard.Core;
using StreamBoard.Features.Updater.Models;
using StreamBoard.Features.Updater.Services;

namespace StreamBoard.Features.Updater.ViewModels
{
    public partial class UpdaterViewModel : ObservableObject
    {
        private readonly AppInfoService _appInfoService;

        public UpdaterViewModel(AppInfoService appInfo)
        {
            _appInfoService = appInfo;
        }

        public AppInfoModel AppInfo => _appInfoService.AppInfo;
    }
}