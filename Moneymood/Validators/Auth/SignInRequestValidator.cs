using FluentValidation;
using MoneyMood.Dtos.Auth;

namespace MoneyMood.Validators.Auth;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email requis")
            .EmailAddress().WithMessage("Format d'email invalide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mot de passe requis");
    }
}