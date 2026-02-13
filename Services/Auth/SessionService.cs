using System.Security.Cryptography;
using MoneyMood.Data;
using MoneyMood.Dtos.Auth;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.Auth;

public class SessionService(AppDbContext context) : ISessionService
{

    private static string GenerateSessionToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes);
    }

    public async Task<string> CreateSessionAsync(string userId)
    {
        var token = GenerateSessionToken();
        var session = new Session
        {
            UserId = userId,
            Id = token,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<SessionInfo?> ValidateSessionAsync(string sessionId)
    {
        var session = await context.Sessions.FindAsync(sessionId);

        if (session is null)
            return null;

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            context.Sessions.Remove(session);
            await context.SaveChangesAsync();
            return null;
        }

        var shouldRefresh = session.ExpiresAt < DateTime.UtcNow.AddDays(7);
        if (shouldRefresh)
        {
            session.ExpiresAt = DateTime.UtcNow.AddDays(30);
            await context.SaveChangesAsync();
        }

        
        return new SessionInfo
        {
            UserId = session.UserId,
            Session = session.Id,
            ShouldRefresh = shouldRefresh
        };
    }

}