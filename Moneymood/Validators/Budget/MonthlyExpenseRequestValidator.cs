using FluentValidation;
using MoneyMood.Dtos.Budget;
using MoneyMood.Validators.Expense;

namespace MoneyMood.Validators.Budget;

public class MonthlyExpenseRequestValidator : AbstractValidator<MonthlyExpenseRequest>
{
    public MonthlyExpenseRequestValidator()
    {
        Include(new ExpenseRequestValidator());

        RuleFor(x => x.WeekNumber)
            .InclusiveBetween(1, 5).WithMessage("Numéro de semaine non valide");

    }
}