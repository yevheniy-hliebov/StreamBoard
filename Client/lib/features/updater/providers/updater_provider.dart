import 'package:flutter/foundation.dart';
import 'package:streamtabula/features/settings/providers/settings_provider.dart';
import 'package:streamtabula/features/updater/models/app_info.dart';
import 'package:streamtabula/features/updater/models/github_release_info.dart';
import 'package:streamtabula/features/updater/services/update_service.dart';

enum UpdateDialogState { loading, upToDate, updateAvailable, downloading }

class UpdaterProvider extends ChangeNotifier {
  final UpdateService _updateService;

  UpdaterProvider(this._updateService);

  UpdateDialogState _dialogState = UpdateDialogState.loading;
  UpdateDialogState get dialogState => _dialogState;

  GithubReleaseInfo? _latestReleaseInfo;
  GithubReleaseInfo? get latestReleaseInfo => _latestReleaseInfo;

  double _downloadProgress = 0.0;
  double get downloadProgress => _downloadProgress;

  String _downloadStatusText = "Preparing to download...";
  String get downloadStatusText => _downloadStatusText;

  Future<void> performManualUpdateCheck(
    AppInfo appInfo,
    bool receiveBeta,
  ) async {
    _dialogState = UpdateDialogState.loading;
    _latestReleaseInfo = null;
    notifyListeners();

    await Future.delayed(const Duration(milliseconds: 600));

    _latestReleaseInfo = await _updateService.checkForUpdates(
      appInfo: appInfo,
      receiveBetaUpdates: receiveBeta,
    );

    if (_latestReleaseInfo != null) {
      _dialogState = UpdateDialogState.updateAvailable;
    } else {
      _dialogState = UpdateDialogState.upToDate;
    }
    notifyListeners();
  }

  Future<bool> checkForUpdatesOnStartup(
    AppInfo appInfo,
    SettingsProvider settings,
  ) async {
    try {
      final releaseInfo = await _updateService.checkForUpdates(
        appInfo: appInfo,
        receiveBetaUpdates: settings.receiveBetaUpdates,
      );

      if (releaseInfo != null) {
        if (settings.skippedVersion == releaseInfo.tagName) {
          return false;
        }

        _latestReleaseInfo = releaseInfo;
        _dialogState = UpdateDialogState.updateAvailable;
        notifyListeners();

        return true;
      }
      return false;
    } catch (e) {
      debugPrint("Startup update check failed: $e");
      return false;
    }
  }

  Future<void> startUpdateProcess() async {
    if (_latestReleaseInfo == null) return;

    _dialogState = UpdateDialogState.downloading;
    _downloadProgress = 0.0;
    _downloadStatusText = "Downloading update apk...";
    notifyListeners();

    try {
      final apkPath = await _updateService.downloadUpdateApk(
        releaseInfo: _latestReleaseInfo!,
        onProgress: (percent) {
          _downloadProgress = percent;
          if (percent >= 100) {
            _downloadStatusText = "Preparing to install...";
          }
          notifyListeners();
        },
      );

      await Future.delayed(const Duration(milliseconds: 500));

      await _updateService.installUpdate(apkPath);
    } catch (e) {
      if (kDebugMode) {
        print("[Updater] Error: $e");
      }
      _dialogState = UpdateDialogState.updateAvailable;
      notifyListeners();
      rethrow;
    }
  }
}
