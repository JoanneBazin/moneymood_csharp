using MoneyMood.Interfaces;

namespace MoneyMood.Models;

public class Expense : IHasAmount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Cashed { get; set; } = false;
    public DateTime? CashedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? MonthlyBudgetId { get; set; }
    public MonthlyBudget? MonthlyBudget { get; set; }
    public int? WeekNumber { get; set;}

    public Guid? SpecialBudgetId { get; set; }
    public SpecialBudget? SpecialBudget { get; set; }
    public Guid? SpecialCategoryId { get; set; }
    public SpecialCategory? SpecialCategory { get; set; }
}