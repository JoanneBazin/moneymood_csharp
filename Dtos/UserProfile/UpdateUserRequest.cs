namespace MoneyMood.Dtos.UserProfile;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? EnabledExpenseValidation { get; set; }
}