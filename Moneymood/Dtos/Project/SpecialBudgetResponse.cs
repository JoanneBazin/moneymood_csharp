using MoneyMood.Dtos.Category;

namespace MoneyMood.Dtos.Project;

public class SpecialBudgetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal RemainingBudget { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public IEnumerable<SpecialExpenseResponse> Expenses { get; set; } = [];
    public IEnumerable<CategoryResponse> Categories { get; set; } = [];
}