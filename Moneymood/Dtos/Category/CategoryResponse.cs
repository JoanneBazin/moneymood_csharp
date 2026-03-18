using MoneyMood.Dtos.Project;

namespace MoneyMood.Dtos.Category;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SpecialBudgetId { get; set; }
    public IEnumerable<SpecialExpenseResponse> Expenses { get; set; } = [];
}