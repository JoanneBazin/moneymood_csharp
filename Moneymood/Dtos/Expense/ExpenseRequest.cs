using MoneyMood.Interfaces;

namespace MoneyMood.Dtos.Expense;

public class ExpenseRequest : IHasAmount
{
    private string _name = string.Empty;
    public string Name 
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty;
    }
    public decimal Amount { get; set; }
}