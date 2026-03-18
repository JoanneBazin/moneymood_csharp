using MoneyMood.Dtos.Expense;

namespace MoneyMood.Dtos.Project;

public class SpecialExpenseResponse : ExpenseResponse
{
    public Guid? SpecialCategoryId { get; set;}
    public DateTime CreatedAt { get; set;}
}