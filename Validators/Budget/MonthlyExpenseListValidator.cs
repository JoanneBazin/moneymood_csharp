using FluentValidation;
using MoneyMood.Dtos.Budget;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Budget;

public class MonthlyExpenseListValidator(IValidator<MonthlyExpenseRequest> validator): CollectionValidator<MonthlyExpenseRequest>(validator)
{
}