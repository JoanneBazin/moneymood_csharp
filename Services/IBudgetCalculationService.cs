
using MoneyMood.Data;
using MoneyMood.Interfaces;

namespace MoneyMood.Services.Budget;

public interface IBudgetCalculationService
{
    decimal CalculateMonthlyRemainingBudget(IEnumerable<IHasAmount> incomes, IEnumerable<IHasAmount> charges, IEnumerable<IHasAmount>? expenses = null);
    decimal CalculateWeeklyBudget(decimal remainingBudget, int numberOfWeeks);
    Task<decimal> UpdateMonthlyRemainingBudgetAsync(Guid budgetId, AppDbContext context);
    Task<decimal> UpdateWeeklyBudgetAsync(Guid budgetId, AppDbContext context);
}