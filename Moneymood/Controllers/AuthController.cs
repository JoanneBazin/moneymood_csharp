using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Auth;
using MoneyMood.Exceptions;
using MoneyMood.Services.Auth;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService service, IHostEnvironment env) : BaseController
{
    [AllowAnonymous]    
    [HttpPost("signup")]
    public async Task<ActionResult<UserResponse>> SignUp(SignUpRequest request)
    {
        var result = await service.SignUpAsync(request);

        var isTestEnv = env.EnvironmentName == "Testing";

        Response.Cookies.Append("session", result.SessionToken.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = !isTestEnv,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromDays(30)
        });
        return Ok(result.User);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> SignIn(SignInRequest request)
    {
        var result = await service.SignInAsync(request);

        var isTestEnv = env.EnvironmentName == "Testing";

        Response.Cookies.Append("session", result.SessionToken.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = !isTestEnv,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromDays(30)
        });
        return Ok(result.User);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<Guid>> Logout()
    {
        var sessionToken = Request.Cookies["session"];
        if (sessionToken is null || !Guid.TryParse(sessionToken, out var token))
            throw ApiException.Unauthorized("Session non valide");

        
        await service.LogoutAsync(token);

        Response.Cookies.Delete("session");
        return Ok("Déconnexion réussie");
    }

    [HttpGet("session")]
    public async Task<ActionResult<Guid>> GetSession()
    {
       var userId = GetUserId();
       var user = await service.GetSessionAsync(userId);
        return Ok(user);
    }
}