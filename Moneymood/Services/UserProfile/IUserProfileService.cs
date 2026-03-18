using MoneyMood.Dtos.Auth;
using MoneyMood.Dtos.UserProfile;

namespace MoneyMood.Services.UserProfile;

public interface IUserProfileService
{
    Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request);
}