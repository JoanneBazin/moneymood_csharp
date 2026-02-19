namespace MoneyMood.Models;

public class SpecialBudget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal RemainingBudget { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Expense> Expenses { get; set; } = [];
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}