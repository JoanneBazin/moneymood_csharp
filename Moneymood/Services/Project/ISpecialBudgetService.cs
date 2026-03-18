using MoneyMood.Dtos.Project;

namespace MoneyMood.Services.Project;

public interface ISpecialBudgetService
{
    Task<SpecialBudgetResponse> CreateSpecialBudgetAsync(Guid userId, SpecialBudgetRequest request);
    Task<IEnumerable<SpecialBudgetListResponse>> GetAllSpecialBudgetsAsync(Guid userId);
    Task<SpecialBudgetResponse> GetSpecialBudgetByIdAsync(Guid userId, Guid budgetId);
    Task<SpecialBudgetResponse> UpdateSpecialBudgetAsync(Guid userId, Guid budgetId, SpecialBudgetRequest request);
    Task<DeletedSpecialBudgetResponse> DeleteSpecialBudgetAsync(Guid userId, Guid budgetId);
    
}