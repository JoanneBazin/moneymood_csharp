namespace MoneyMood.Dtos.Auth;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool? EnabledExpenseValidation { get; set; }
}