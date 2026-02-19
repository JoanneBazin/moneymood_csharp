using MoneyMood.Interfaces;

namespace MoneyMood.Models;

public class MonthlyEntry : IHasAmount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public EntryType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid MonthlyBudgetId { get; set; }
    public MonthlyBudget MonthlyBudget { get; set; } = null!;
}