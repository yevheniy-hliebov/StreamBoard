namespace StreamTabula.Features.Servers.Services;

public class RouteMatcher
{
    public static bool TryMatch(string requestPath, string routeTemplate, out Dictionary<string, string> parameters)
    {
        parameters = [];
        
        var reqSegments = requestPath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        var tplSegments = routeTemplate.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (reqSegments.Length != tplSegments.Length) 
            return false;

        for (int i = 0; i < tplSegments.Length; i++)
        {
            if (tplSegments[i].StartsWith("{") && tplSegments[i].EndsWith("}"))
            {
                var key = tplSegments[i].Trim('{', '}');
                parameters[key] = Uri.UnescapeDataString(reqSegments[i]);
            }
            else if (!string.Equals(reqSegments[i], tplSegments[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}