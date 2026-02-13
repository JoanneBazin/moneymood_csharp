using MoneyMood.Dtos.Auth;

namespace MoneyMood.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> SignUpAsync(SignUpRequest request);
    Task<AuthResponse> SignInAsync(SignInRequest request);
    Task LogoutAsync(string sessionToken);
    Task<UserResponse> GetSessionAsync(string userId);
}