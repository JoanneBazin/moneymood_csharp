using FluentValidation;
using MoneyMood.Dtos.Project;
using MoneyMood.Validators.Expense;

namespace MoneyMood.Validators.Project;

public class SpecialExpenseRequestValidator : AbstractValidator<SpecialExpenseRequest>
{
    public SpecialExpenseRequestValidator()
    {
        Include(new ExpenseRequestValidator());
    }
}