using FluentValidation;
using MoneyMood.Dtos.Budget;
using MoneyMood.Validators.Entry;

namespace MoneyMood.Validators.Budget;

public class CreateMonthlyBudgetRequestValidator : AbstractValidator<CreateMonthlyBudgetRequest>
{
    public CreateMonthlyBudgetRequestValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Le mois doit être compris entre 1 et 12");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2025).WithMessage("L'année doit être supérieure ou égale à 2025");

        RuleFor(x => x.NumberOfWeeks)
            .InclusiveBetween(4, 5).WithMessage("Nombre de semaines invalide");
        
        RuleForEach(x => x.Incomes)
            .SetValidator(new EntryRequestValidator());

        RuleForEach(x => x.Charges)
            .SetValidator(new EntryRequestValidator());

    }
}