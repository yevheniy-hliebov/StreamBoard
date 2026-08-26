using Microsoft.AspNetCore.Mvc;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Servers.Models;
using System.IO;

namespace StreamTabula.Features.Servers.Controllers;

[ApiController]
[Route("twitch")]
public class TwitchAuthController(ITwitchAccountsGateway gateway) : ControllerBase
{

    [HttpGet("{role}")]
    public IActionResult ServeAuthPage(string role)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "integrations", "twitch", "twitch_auth.html");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Auth template not found.");
        }

        return PhysicalFile(filePath, "text/html");
    }

    [HttpPost("{role}")]
    public async Task<IActionResult> HandleTokenSubmit(string role, [FromBody] TwitchAuthResponse payload)
    {
        try
        {
            if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
                return BadRequest("Invalid payload or missing access token.");

            if (!Enum.TryParse<TwitchAccountRole>(role, ignoreCase: true, out var accountRole))
            {
                return BadRequest($"Unknown role: {role}");
            }

            var scopesList = (payload.Scope ?? "")
                .Split([' ', '+'], StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string tokenType = string.IsNullOrEmpty(payload.TokenType)
                ? "Bearer"
                : char.ToUpper(payload.TokenType[0]) + payload.TokenType.Substring(1);

            var authContext = new TwitchAuthContext(payload.AccessToken, tokenType, scopesList);
            string state = payload.State ?? string.Empty;

            switch (accountRole)
            {
                case TwitchAccountRole.Broadcaster:
                    await gateway.Broadcaster.Authenticator.FinalizeLogin(authContext, state);
                    break;
                case TwitchAccountRole.Bot:
                    await gateway.Bot.Authenticator.FinalizeLogin(authContext, state);
                    break;
                default:
                    return BadRequest($"Unsupported role: {accountRole}");
            }

            return Ok("Data received");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}