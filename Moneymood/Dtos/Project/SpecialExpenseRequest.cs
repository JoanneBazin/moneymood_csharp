using MoneyMood.Dtos.Expense;

namespace MoneyMood.Dtos.Project;

public class SpecialExpenseRequest : ExpenseRequest
{
    public Guid? SpecialCategoryId { get; set;}
}