namespace StreamTabula.Core.Constants
{
    public static class UpdateConstants
    {
        public const string GitHubOwner = "yevheniy-hliebov";
        public const string GitHubRepo = "StreamTabula";
        public const string GitHubApiBaseUrl = "https://api.github.com/repos";
        public static string ReleaseAssetName(string version) => $"StreamTabula-Windows-{version}.zip";
    }
}