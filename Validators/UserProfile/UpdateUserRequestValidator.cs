using MoneyMood.Dtos.UserProfile;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.UserProfile;

public class UpdateUserRequestValidator : AuthBaseValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        SetupEmailRules(x => x.Email, required: false);
        SetupNameRules(x => x.Name, required: false);
    }
}