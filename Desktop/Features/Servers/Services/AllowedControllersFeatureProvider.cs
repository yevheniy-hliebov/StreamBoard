using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace StreamTabula.Features.Servers.Services;

public class AllowedControllersFeatureProvider(Type[] allowedControllers) : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        return allowedControllers.Contains(typeInfo.AsType());
    }
}