namespace MoneyMood.Models;

public class MonthlyEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public EntryType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string MonthlyBudgetId { get; set; } = string.Empty;
    public MonthlyBudget MonthlyBudget { get; set; } = null!;
}