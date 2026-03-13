using MoneyMood.Dtos.Expense;
using MoneyMood.Validators.Shared;

namespace MoneyMood.Validators.Expense;

public class ExpenseRequestValidator : EntryBaseValidator<ExpenseRequest>
{
    public ExpenseRequestValidator()
    {
        SetupEntryNameRules(x => x.Name);
        SetupEntryAmountRules(x => x.Amount);
    }
}