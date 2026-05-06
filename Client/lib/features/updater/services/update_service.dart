import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:pub_semver/pub_semver.dart';
import 'package:streamtabula/core/constants/update_constants.dart';
import 'package:streamtabula/features/updater/models/app_info.dart';
import 'package:streamtabula/features/updater/models/github_asset_info.dart';
import 'package:streamtabula/features/updater/models/github_release_info.dart';
import 'package:path_provider/path_provider.dart';
import 'package:open_filex/open_filex.dart';

class UpdateService {
  final Dio _dio;

  static const String _owner = UpdateConstants.gitHubOwner;
  static const String _repo = UpdateConstants.gitHubRepo;
  static const String _githubUrl = UpdateConstants.gitHubApiBaseUrl;

  static const String _baseUrl = '$_githubUrl/$_owner/$_repo/releases';

  UpdateService() : _dio = Dio();

  Future<GithubReleaseInfo?> checkForUpdates({
    required AppInfo appInfo,
    bool receiveBetaUpdates = false,
  }) async {
    try {
      final response = await _dio.get(
        _baseUrl,
        options: Options(
          headers: {
            'User-Agent':
                '${appInfo.appName.replaceAll(" ", "")}/$appInfo.currentVersion',
          },
        ),
      );

      if (response.statusCode != 200) return null;

      final List<dynamic> data = response.data;
      final releases = data
          .map((json) => GithubReleaseInfo.fromJson(json))
          .toList();

      if (releases.isEmpty) return null;

      final validReleases = receiveBetaUpdates
          ? releases
          : releases.where((r) => !r.prerelease).toList();

      if (validReleases.isEmpty) return null;

      final latestRelease = validReleases.first;

      final currentVer = Version.parse(appInfo.currentVersion);
      final remoteVer = Version.parse(latestRelease.cleanVersion);

      if (remoteVer > currentVer) {
        return latestRelease;
      }

      return null;
    } on DioException catch (e) {
      if (kDebugMode) {
        print('[UpdateService] Error checking for updates: ${e.message}');
      }
      return null;
    } catch (e) {
      if (kDebugMode) {
        print('[UpdateService] Unexpected error: $e');
      }
      return null;
    }
  }

  Future<String> downloadUpdateApk({
    required GithubReleaseInfo releaseInfo,
    required Function(double) onProgress,
  }) async {
    try {
      final asset = releaseInfo.assets.cast<GithubAssetInfo?>().firstWhere(
        (a) => a != null && a.name.endsWith('.apk'),
        orElse: () => null,
      );

      if (asset == null) {
        throw Exception("No .apk file found in this release");
      }

      final tempDir = await getTemporaryDirectory();

      final assetName = UpdateConstants.releaseAssetName(releaseInfo.tagName);

      final savePath = '${tempDir.path}/$assetName';

      final file = File(savePath);
      if (await file.exists()) {
        await file.delete();
      }

      await _dio.download(
        asset.browserDownloadUrl,
        savePath,
        onReceiveProgress: (received, total) {
          if (total != -1) {
            onProgress((received / total) * 100);
          }
        },
      );

      return savePath;
    } on DioException catch (e) {
      if (kDebugMode) {
        print('[UpdateService] Error loading via Dio: ${e.message}');
      }
      rethrow;
    } catch (e) {
      if (kDebugMode) {
        print('[UpdateService] File saving error: $e');
      }
      rethrow;
    }
  }

  Future<void> installUpdate(String apkPath) async {
    try {
      final result = await OpenFilex.open(apkPath);

      if (result.type != ResultType.done) {
        throw Exception('Installation failed to start.: ${result.message}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('[UpdateService] Installation error: $e');
      }
      rethrow;
    }
  }
}
