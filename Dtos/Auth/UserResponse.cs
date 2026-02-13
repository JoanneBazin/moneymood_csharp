namespace MoneyMood.Dtos.Auth;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool? EnabledExpenseValidation { get; set; }
}