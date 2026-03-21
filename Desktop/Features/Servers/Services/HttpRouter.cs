using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamBoard.Features.Servers.Services
{
    public class HttpRouter
    {
        private readonly List<IHttpController> _controllers;

        public HttpRouter(IEnumerable<IHttpController> controllers)
        {
            _controllers = controllers
                .OrderByDescending(c => c.RoutePrefix.Length)
                .ToList();
        }

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