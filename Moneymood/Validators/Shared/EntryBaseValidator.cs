using System.Linq.Expressions;
using FluentValidation;

namespace MoneyMood.Validators.Shared;
public abstract class EntryBaseValidator<T> : AbstractValidator<T>
{
    protected void SetupEntryNameRules(Expression<Func<T, string>> nameSelector)
    {
        RuleFor(nameSelector)
            .NotEmpty().WithMessage("Le nom est requis")
            .MaximumLength(100).WithMessage("Le nom est trop long");
            
    }
    protected void SetupEntryAmountRules(Expression<Func<T, decimal>> amountSelector)
    {
        RuleFor(amountSelector)
            .NotEmpty().WithMessage("Le montant est requis")
            .GreaterThan(0).WithMessage("Veuillez saisir un montant positif valide");
            
    }

}