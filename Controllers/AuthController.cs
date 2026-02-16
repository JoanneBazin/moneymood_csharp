using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Auth;
using MoneyMood.Exceptions;
using MoneyMood.Services.Auth;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService service) : BaseController
{
    [AllowAnonymous]    
    [HttpPost("signup")]
    public async Task<ActionResult<UserResponse>> SignUp(SignUpRequest request)
    {
        var result = await service.SignUpAsync(request);

        Response.Cookies.Append("session", result.SessionToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
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

        Response.Cookies.Append("session", result.SessionToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromDays(30)
        });
        return Ok(result.User);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<string>> Logout()
    {
        var sessionToken = Request.Cookies["session"];
        if (sessionToken is null)
            throw ApiException.Unauthorized("Session non trouvé");

        
        await service.LogoutAsync(sessionToken);

        Response.Cookies.Delete("session");
        return Ok("Déconnexion réussie");
    }

    [HttpGet("session")]
    public async Task<ActionResult<string>> GetSession()
    {
       var userId = GetUserId();
       var user = await service.GetSessionAsync(userId);
        return Ok(user);
    }
}