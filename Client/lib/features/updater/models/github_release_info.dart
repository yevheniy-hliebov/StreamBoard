import 'package:streamboard/features/updater/models/github_asset_info.dart';

class GithubReleaseInfo {
  final String url;
  final String name;
  final String tagName;
  final String body;
  final bool draft;
  final bool prerelease;
  final String createdAt;
  final String publishedAt;
  final List<GithubAssetInfo> assets;

  const GithubReleaseInfo({
    required this.url,
    required this.name,
    required this.tagName,
    required this.body,
    required this.draft,
    required this.prerelease,
    required this.createdAt,
    required this.publishedAt,
    required this.assets,
  });

  factory GithubReleaseInfo.fromJson(Map<String, dynamic> json) {
    return GithubReleaseInfo(
      url: json['url'] ?? '',
      name: json['name'] ?? '',
      tagName: json['tag_name'] ?? '',
      body: json['body'] ?? '',
      draft: json['draft'] ?? false,
      prerelease: json['prerelease'] ?? false,
      createdAt: json['created_at'] ?? '',
      publishedAt: json['published_at'] ?? '',
      assets: (json['assets'] as List? ?? [])
          .map((assetJson) => GithubAssetInfo.fromJson(assetJson))
          .toList(),
    );
  }

  String get cleanVersion {
    return tagName.startsWith("v") ? tagName.substring(1) : tagName;
  }
}
