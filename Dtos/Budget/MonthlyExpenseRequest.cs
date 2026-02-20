using MoneyMood.Dtos.Expense;

namespace MoneyMood.Dtos.Budget;

public class MonthlyExpenseRequest : ExpenseRequest
{
    public int WeekNumber { get; set;}
}