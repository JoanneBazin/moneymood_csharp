using FluentValidation;
using MoneyMood.Dtos.Project;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Project;

public class SpecialExpenseListValidator(IValidator<SpecialExpenseRequest> validator): CollectionValidator<SpecialExpenseRequest>(validator)
{
}