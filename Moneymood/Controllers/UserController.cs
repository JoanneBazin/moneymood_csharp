using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Auth;
using MoneyMood.Dtos.UserProfile;
using MoneyMood.Services.UserProfile;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserProfileService service) : BaseController
{
    [HttpPatch("me")]
    public async Task<ActionResult<UserResponse>> UpdateUser(UpdateUserRequest request)
    {
        return Ok(await service.UpdateUserAsync(GetUserId(), request));
    }
}