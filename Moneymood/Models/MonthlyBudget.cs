namespace MoneyMood.Models;

public class MonthlyBudget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsCurrent { get; set; } = false;
    public decimal RemainingBudget { get; set; }
    public decimal WeeklyBudget { get; set; }
    public int NumberOfWeeks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Expense> Expenses { get; set; } = [];
    public ICollection<MonthlyEntry> MonthlyEntries { get; set; } = [];
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}