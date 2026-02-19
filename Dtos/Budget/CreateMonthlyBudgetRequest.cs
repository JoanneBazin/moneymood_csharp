namespace MoneyMood.Dtos.Budget;

public class CreateMonthlyBudgetRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsCurrent { get; set; }
    public int NumberOfWeeks { get; set; }
    public ICollection<CreateEntryRequest> Incomes { get; set; } = [];
    public ICollection<CreateEntryRequest> Charges { get; set; } = [];
}