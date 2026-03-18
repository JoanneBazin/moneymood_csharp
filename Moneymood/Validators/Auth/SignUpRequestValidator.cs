using MoneyMood.Dtos.Auth;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Auth;

public class SignUpRequestValidator : AuthBaseValidator<SignUpRequest>
{
    public SignUpRequestValidator()
    {
        SetupEmailRules(x => x.Email);
        SetupPasswordRules(x => x.Password);
        SetupNameRules(x => x.Name);     
    }
}