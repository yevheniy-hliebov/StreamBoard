using Microsoft.AspNetCore.Mvc;
using StreamTabula.Features.Servers.Services;

namespace StreamTabula.Features.Servers.Controllers;

[ApiController]
[Route("/")]
public class HomeController(ServerConfigsStorage storage, ILocalIpAddressResolver ipResolver) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var address = ipResolver.Get();
        string responseText = $"Running on {address}:{storage.Current.Local.Port}";
        
        return Content(responseText, "text/plain");
    }
}