using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Auth;
using MoneyMood.Dtos.UserProfile;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.UserProfile;

public class UserProfileService(AppDbContext context) : IUserProfileService
{
    public async Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await context.Users.FindAsync(userId)
        ?? throw ApiException.NotFound("Utilisateur non trouvé");

        if (request.Email != null && request.Email != user.Email)
        {
            var emailExists = await context.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != userId);
                
            if (emailExists) 
                throw ApiException.Conflict("Email déjà utilisé");
            
            user.Email = request.Email;
        }

        if (request.Name != null)
            user.Name = request.Name;
        if (request.EnabledExpenseValidation.HasValue)
            user.EnabledExpenseValidation = request.EnabledExpenseValidation.Value;

        await context.SaveChangesAsync();

        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        EnabledExpenseValidation = user.EnabledExpenseValidation
    };
}