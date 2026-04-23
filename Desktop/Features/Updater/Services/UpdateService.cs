using System.Diagnostics;
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
    }
}