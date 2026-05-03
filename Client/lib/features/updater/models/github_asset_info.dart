class GithubAssetInfo {
  final String name;
  final String contentType;
  final String createdAt;
  final String publishedAt;
  final String browserDownloadUrl;

  const GithubAssetInfo(
    this.name,
    this.contentType,
    this.createdAt,
    this.publishedAt,
    this.browserDownloadUrl,
  );

  factory GithubAssetInfo.fromJson(Map<String, dynamic> json) {
    return GithubAssetInfo(
      json['name'] ?? '',
      json['content_type'] ?? '',
      json['created_at'] ?? '',
      json['updated_at'] ?? '',
      json['browser_download_url'] ?? '',
    );
  }
}
