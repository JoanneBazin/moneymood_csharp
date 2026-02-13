namespace MoneyMood.Dtos.Auth;

public class AuthResponse
{
    public UserResponse User { get; set; } = new UserResponse();
    public string SessionToken { get; set; } = string.Empty;
}