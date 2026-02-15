using System;
using System.Collections.Generic;
using System.Text;

namespace StreamBoard.Features.Servers.Services
{
    public class HttpRouter
    {
        private readonly Dictionary<string, IHttpController> _map;

        public HttpRouter(IEnumerable<IHttpController> controllers)
        {
            _map = controllers.ToDictionary(
                c => Normalize(c.RoutePrefix),
                c => c);
        }

        public IHttpController? Resolve(string path)
        {
            var key = Normalize(path);
            return _map.TryGetValue(key, out var controller)
                ? controller
                : null;
        }

        private static string Normalize(string path)
            => path.TrimEnd('/').ToLowerInvariant();
    }

}
