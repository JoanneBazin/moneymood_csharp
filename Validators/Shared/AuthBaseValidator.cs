using System.Linq.Expressions;
using FluentValidation;

namespace MoneyMood.Validators.Shared;
public abstract class AuthBaseValidator<T> : AbstractValidator<T>
{
    protected void SetupEmailRules(Expression<Func<T, string?>> emailSelector, bool required = true)
    {
        if (required)
        {
            RuleFor(emailSelector)
                .NotEmpty().WithMessage("Email requis");
        }
        RuleFor(emailSelector)
            .EmailAddress().WithMessage("Format d'email invalide")
            .When(x => !string.IsNullOrWhiteSpace(emailSelector.Compile()(x)));
    }

    protected void SetupPasswordRules(Expression<Func<T, string>> passwordSelector)
    {
        RuleFor(passwordSelector)
            .NotEmpty().WithMessage("Mot de passe requis")
            .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères")
            .MaximumLength(100).WithMessage("Le mot de passe est trop long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$").WithMessage("Le mot de passe doit contenir au moins : une minuscule, une majuscule et un chiffre");
    }

    protected void SetupNameRules(Expression<Func<T, string?>> nameSelector, bool required = true)
    {
        if (required)
        {
            RuleFor(nameSelector)
                .NotEmpty().WithMessage("Le nom est requis");
        }
        RuleFor(nameSelector)
            .MaximumLength(100).WithMessage("Le nom est trop long")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'\-]*$").WithMessage("Le nom contient des caractères invalides")
            .When(x => !string.IsNullOrWhiteSpace(nameSelector.Compile()(x)));
    }
}