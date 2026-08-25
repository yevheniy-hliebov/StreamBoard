using Microsoft.AspNetCore.Mvc;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Servers.Models;

namespace StreamTabula.Features.Servers.Controllers;

[ApiController]
[Route("twitch")]
public class TwitchAuthController(ITwitchAccountsGateway gateway) : ControllerBase
{

    [HttpGet("{role}")]
    public IActionResult ServeAuthPage(string role)
    {
        return Content(TwitchAuthTemplates.AuthPageHtml, "text/html");
    }

    [HttpPost("{role}")]
    public async Task<IActionResult> HandleTokenSubmit(string role, [FromBody] TwitchAuthResponse payload)
    {
        try
        {
            if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
                return BadRequest("Invalid payload or missing access token.");

            var scopesList = (payload.Scope ?? "")
                .Split([' ', '+'], StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string tokenType = string.IsNullOrEmpty(payload.TokenType)
                ? "Bearer"
                : char.ToUpper(payload.TokenType[0]) + payload.TokenType.Substring(1);

            var authContext = new TwitchAuthContext(payload.AccessToken, tokenType, scopesList);
            string state = payload.State ?? string.Empty;

            if (role.Equals("broadcaster", StringComparison.OrdinalIgnoreCase))
            {
                await gateway.Broadcaster.Authenticator.FinalizeLogin(authContext, state);
            }
            else if (role.Equals("bot", StringComparison.OrdinalIgnoreCase))
            {
                await gateway.Bot.Authenticator.FinalizeLogin(authContext, state);
            }
            else
            {
                return BadRequest($"Unknown role: {role}");
            }

            return Ok("Data received");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}