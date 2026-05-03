import 'package:package_info_plus/package_info_plus.dart';
import 'package:streamboard/features/updater/models/app_info.dart';

class AppInfoService {
  late final AppInfo appInfo;

  Future<void> init() async {
    final packageInfo = await PackageInfo.fromPlatform();
    final cleanVersion = packageInfo.version.split('+').first;

    appInfo = AppInfo(appName: "StreamBoard", currentVersion: cleanVersion);
  }
}
