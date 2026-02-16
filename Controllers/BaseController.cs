using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MoneyMood.Exceptions;

namespace MoneyMood.Controllers;

public class BaseController : ControllerBase
{
    protected string GetUserId()
    {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw ApiException.Unauthorized("UserId inconnu"); 
       
       return userId;
    }
}