using MoneyMood.Dtos.Entry;

namespace MoneyMood.Dtos.Budget;

public class CreateMonthlyBudgetRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsCurrent { get; set; }
    public int NumberOfWeeks { get; set; }
    public ICollection<EntryRequest> Incomes { get; set; } = [];
    public ICollection<EntryRequest> Charges { get; set; } = [];
}