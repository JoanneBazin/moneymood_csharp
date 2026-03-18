
namespace MoneyMood.Dtos.Auth;

public class SignUpRequest
{
    private string _email = string.Empty;
    private string _name = string.Empty;

    public string Email 
    { 
        get => _email; 
        set => _email = value?.Trim().ToLower() ?? string.Empty; 
    }

    public string Password { get; set; } = string.Empty;

    public string Name
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty; 
    }
}