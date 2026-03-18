namespace MoneyMood.Models;

public class SpecialCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<Expense> Expenses { get; set; } = [];
    public Guid SpecialBudgetId { get; set; }
    public SpecialBudget SpecialBudget { get; set; } = null!;
}