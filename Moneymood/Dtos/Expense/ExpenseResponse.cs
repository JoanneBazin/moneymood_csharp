namespace MoneyMood.Dtos.Expense;

public class ExpenseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Cashed { get; set; } = false;
    
}