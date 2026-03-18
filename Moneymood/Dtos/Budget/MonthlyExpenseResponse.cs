using MoneyMood.Dtos.Expense;

namespace MoneyMood.Dtos.Budget;

public class MonthlyExpenseResponse : ExpenseResponse
{
    public int WeekNumber { get; set;}
}