using MoneyMood.Dtos.Project;

namespace MoneyMood.Dtos.Category;

public class DeletedCategoryResponse
{
    public Guid Id { get; set; }
    public IEnumerable<SpecialExpenseResponse> Expenses { get; set; } = [];
}