using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Auth;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.Auth;

public class AuthService(AppDbContext context, IPasswordHasher hasher, ISessionService sessionService) : IAuthService
{
    public async Task<AuthResponse> SignUpAsync(SignUpRequest request)
    {
        var newUser = new User
        {
            Email = request.Email,
            Password = hasher.Hash(request.Password),
            Name = request.Name
        };
        context.Users.Add(newUser);

        try
        {
          await context.SaveChangesAsync();   
        }
        catch (DbUpdateException)
        {
            throw ApiException.Conflict("Email déjà utilisé");
        }
       

        var sessionToken = await sessionService.CreateSessionAsync(newUser.Id);

        return new AuthResponse
        {
            User = new UserResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.Name,
                EnabledExpenseValidation = newUser.EnabledExpenseValidation
            },
            SessionToken = sessionToken
        };
    }

    public async Task<AuthResponse> SignInAsync(SignInRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !hasher.Verify(request.Password, user.Password))
        {
            throw ApiException.Unauthorized("Email ou mot de passe incorrect");
        }

        var sessionToken = await sessionService.CreateSessionAsync(user.Id);

        return new AuthResponse
        {
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                EnabledExpenseValidation = user.EnabledExpenseValidation
            },
            SessionToken = sessionToken
        };
    }

    public async Task LogoutAsync(string sessionToken)
    {
        var session = await context.Sessions.FindAsync(sessionToken);
        if (session is null)
            throw ApiException.Unauthorized("Session non trouvé");
        
        context.Sessions.Remove(session);
        await context.SaveChangesAsync();
    }

    public async Task<UserResponse> GetSessionAsync(string userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user is null)
            throw ApiException.NotFound("Utilisateur non trouvé");

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            EnabledExpenseValidation = user.EnabledExpenseValidation,
        };
    }
}
