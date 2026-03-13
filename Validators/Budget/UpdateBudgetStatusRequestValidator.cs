using FluentValidation;
using MoneyMood.Dtos.Budget;

namespace MoneyMood.Validators.Budget;

public class UpdateBudgetStatusRequestValidator : AbstractValidator<UpdateBudgetStatusRequest>
{
    public UpdateBudgetStatusRequestValidator()
    {
        RuleFor(x => x.IsCurrent)
            .NotNull().WithMessage("IsCurrent est requis");

    }
}