using MoneyMood.Dtos.Auth;

namespace MoneyMood.Services.Auth;

public interface ISessionService
{
    Task<string> CreateSessionAsync(string userId);
    Task<SessionInfo?> ValidateSessionAsync(string sessionId);
    
}