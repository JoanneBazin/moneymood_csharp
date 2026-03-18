using MoneyMood.Dtos.Auth;

namespace MoneyMood.Services.Auth;

public interface ISessionService
{
    Task<Guid> CreateSessionAsync(Guid userId);
    Task<SessionInfo?> ValidateSessionAsync(Guid sessionId);
    
}