using MoneyMood.Dtos.Auth;

namespace MoneyMood.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> SignUpAsync(SignUpRequest request);
    Task<AuthResponse> SignInAsync(SignInRequest request);
    Task LogoutAsync(Guid sessionToken);
    Task<UserResponse> GetSessionAsync(Guid userId);
}