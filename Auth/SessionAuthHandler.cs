using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MoneyMood.Services.Auth;

namespace MoneyMood.Auth;

public class SessionAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISessionService sessionService
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Cookies["session"];
        if (token is null)
        {
            Response.Cookies.Delete("session");
            return AuthenticateResult.Fail("Pas de cookie de session");
        }

        var result = await sessionService.ValidateSessionAsync(token);
        if (result is null)
        {
            Response.Cookies.Delete("session");
            return AuthenticateResult.Fail("Pas de cookie de session");
        }

        if (result.ShouldRefresh)
        {
            Response.Cookies.Append("session", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromDays(30)
        });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.UserId)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}