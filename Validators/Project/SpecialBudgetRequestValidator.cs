using FluentValidation;
using MoneyMood.Dtos.Project;

namespace MoneyMood.Validators.Project;

public class SpecialBudgetRequestValidator : AbstractValidator<SpecialBudgetRequest>
{
    public SpecialBudgetRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom est requis")
            .MaximumLength(30).WithMessage("Le nom est trop long");

        RuleFor(x => x.TotalBudget)
            .NotEmpty().WithMessage("Le montant est requis")
            .GreaterThanOrEqualTo(0).WithMessage("Veuillez saisir un montant valide");

    }
}