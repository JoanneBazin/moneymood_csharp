using MoneyMood.Dtos.Budget;

namespace MoneyMood.Services.Budget;

public interface IMonthlyBudgetService
{
    Task<MonthlyBudgetResponse> CreateMonthlyBudgetAsync(Guid userId, CreateMonthlyBudgetRequest request);
    Task<HistoryResponse> GetMonthlyBudgetByDateAsync(Guid userId, int month, int year);
    Task<MonthlyBudgetResponse?> GetCurrentMonthlyBudgetAsync(Guid userId);
    Task<MonthlyBudgetResponse> GetMonthlyBudgetByIdAsync(Guid userId, Guid budgetId);
    Task<IEnumerable<HistoryResponse>> GetLastBudgetsAsync(Guid userId);
    Task<MonthlyBudgetResponse> UpdateMonthlyBudgetStatusAsync(Guid userId, Guid budgetId, UpdateBudgetStatusRequest request);
    Task<DeletedBudgetResponse> DeleteMonthlyBudgetAsync(Guid userId, Guid budgetId);
}