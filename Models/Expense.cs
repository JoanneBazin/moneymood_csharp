namespace MoneyMood.Models;

public class Expense
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Cashed { get; set; } = false;
    public DateTime? CashedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? MonthlyBudgetId { get; set; }
    public MonthlyBudget? MonthlyBudget { get; set; }
    public int? WeekNumber { get; set;}

    public string? SpecialBudgetId { get; set; }
    public SpecialBudget? SpecialBudget { get; set; }
    public string? SpecialCategoryId { get; set; }
    public SpecialCategory? SpecialCategory { get; set; }
}