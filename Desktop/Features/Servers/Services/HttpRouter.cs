namespace StreamTabula.Features.Servers.Services
{
    public class HttpRouter(IEnumerable<IHttpController> controllers)
    {
        private readonly List<IHttpController> _controllers = controllers
                .OrderByDescending(c => c.RoutePrefix.Length)
                .ToList();

        public IHttpController? Resolve(string path)
        {
            var normalizedPath = Normalize(path);

            return _controllers.FirstOrDefault(c =>
                normalizedPath.StartsWith(Normalize(c.RoutePrefix), StringComparison.OrdinalIgnoreCase));
        }

        private static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path)) return "/";
            var normalized = path.ToLowerInvariant();
            return normalized == "/" ? normalized : normalized.TrimEnd('/');
        }
    }
}