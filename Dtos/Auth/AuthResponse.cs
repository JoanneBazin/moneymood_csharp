namespace MoneyMood.Dtos.Auth;

public class AuthResponse
{
    public UserResponse User { get; set; } = new UserResponse();
    public Guid SessionToken { get; set; }
}