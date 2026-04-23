using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using NuGet.Versioning;
using StreamBoard.Core.Constants;
using StreamBoard.Features.Updater.Models;

namespace StreamBoard.Features.Updater.Services
{
    public class UpdateService
    {
        private readonly HttpClient _httpClient;

        public UpdateService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<GithubReleaseInfo?> CheckForUpdatesAsync(AppInfoModel appInfo, bool receiveBetaUpdates = false)
        {
            string url = $"{UpdateConstants.GitHubApiBaseUrl}/{UpdateConstants.GitHubOwner}/{UpdateConstants.GitHubRepo}/releases";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.UserAgent.Add(new ProductInfoHeaderValue(appInfo.AppName.Replace(" ", ""), appInfo.CurrentVersion));

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseText = await response.Content.ReadAsStringAsync();

                var releases = JsonSerializer.Deserialize<List<GithubReleaseInfo>>(responseText);
                if (releases == null || releases.Count == 0) return null;

                var validReleases = receiveBetaUpdates ? releases : [.. releases.Where(r => !r.Prerelease)];

                if (validReleases.Count == 0) return null;

                var latestRelease = validReleases.First();

                if (SemanticVersion.TryParse(appInfo.CurrentVersion, out var currentVer) &&
                    SemanticVersion.TryParse(latestRelease.CleanVersion, out var remoteVer))
                {
                    if (remoteVer > currentVer)
                    {
                        return latestRelease;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateService] Error checking for updates: {ex.Message}");
                return null;
            }
        }

        public async Task<string> DownloadUpdateArchiveAsync(
            GithubReleaseInfo releaseInfo,
            AppInfoModel appInfo,
            IProgress<double> progress)
        {
            var asset = releaseInfo.Assets?.FirstOrDefault(a => a.Name.EndsWith(".zip"));
            if (asset == null)
                throw new Exception("No .zip asset found in the release.");

            string tempPath = Path.GetTempPath();
            string zipPath = Path.Combine(tempPath, $"StreamBoard_Windows_{releaseInfo.TagName}.zip");

            using var request = new HttpRequestMessage(HttpMethod.Get, asset.BrowserDownloadUrl);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(appInfo.AppName.Replace(" ", ""), appInfo.CurrentVersion));

            try
            {
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 1L;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                var isMoreToRead = true;
                var bytesReadTotal = 0L;

                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        progress.Report(100);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    bytesReadTotal += bytesRead;

                    progress.Report((double)bytesReadTotal / totalBytes * 100);
                }
                while (isMoreToRead);

                return zipPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateService] Error downloading update: {ex.Message}");
                throw;
            }
        }
    }
}