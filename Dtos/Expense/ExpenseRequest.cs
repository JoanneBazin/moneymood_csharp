using MoneyMood.Interfaces;

namespace MoneyMood.Dtos.Expense;

public class ExpenseRequest : IHasAmount
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int WeekNumber { get; set; }
}