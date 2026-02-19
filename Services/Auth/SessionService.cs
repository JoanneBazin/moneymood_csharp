using MoneyMood.Data;
using MoneyMood.Dtos.Auth;
using MoneyMood.Models;

namespace MoneyMood.Services.Auth;

public class SessionService(AppDbContext context) : ISessionService
{

    public async Task<Guid> CreateSessionAsync(Guid userId)
    {
        var session = new Session
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
        return session.Id;
    }

    public async Task<SessionInfo?> ValidateSessionAsync(Guid sessionId)
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