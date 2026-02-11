namespace MoneyMood.Models;

public class SpecialCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    public ICollection<Expense> Expenses { get; set; } = [];
    public string SpecialBudgetId { get; set; } = string.Empty;
    public SpecialBudget SpecialBudget { get; set; } = null!;
}