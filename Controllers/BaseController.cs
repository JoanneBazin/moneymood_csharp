using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MoneyMood.Exceptions;

namespace MoneyMood.Controllers;

public class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw ApiException.Unauthorized("UserId inconnu"); 

       if (!Guid.TryParse(userId, out var result))
        throw ApiException.BadRequest("Id de session non valide");
       
       return result;
    }
}