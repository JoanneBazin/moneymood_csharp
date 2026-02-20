using MoneyMood.Dtos.Expense;

namespace MoneyMood.Dtos.Budget;

public class MonthlyBudgetResponse
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsCurrent { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal WeeklyBudget { get; set; }
    public int NumberOfWeeks { get; set; }
    public IEnumerable<ExpenseResponse> Expenses { get; set; } = [];
    public IEnumerable<EntryResponse> Incomes { get; set; } = [];
    public IEnumerable<EntryResponse> Charges { get; set; } = [];
}