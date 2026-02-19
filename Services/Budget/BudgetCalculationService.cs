using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Exceptions;
using MoneyMood.Interfaces;
using MoneyMood.Models;

namespace MoneyMood.Services.Budget;

public class BudgetCalculationService : IBudgetCalculationService
{
    public decimal CalculateMonthlyRemainingBudget(IEnumerable<IHasAmount> incomes, IEnumerable<IHasAmount> charges, IEnumerable<IHasAmount>? expenses = null)
    {
        var totalIncomes = incomes.Sum(x => x.Amount);
        var totalCharges = charges.Sum(x => x.Amount);
        var totalExpenses = expenses?.Sum(x => x.Amount) ?? 0;

        return totalIncomes - totalCharges - totalExpenses;
    }

    public decimal CalculateWeeklyBudget(decimal remainingBudget, int numberOfWeeks)
    {
        if (numberOfWeeks <= 0) throw ApiException.BadRequest("Le nombre de semaines doit être supérieur à 0");
        return Math.Round(remainingBudget / numberOfWeeks, 0);
    }

    public async Task<decimal> UpdateMonthlyRemainingBudgetAsync(Guid budgetId, AppDbContext context)
    {
        var budget = await context.MonthlyBudgets
            .Include(b => b.MonthlyEntries)
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.Id == budgetId)
            ?? throw ApiException.NotFound("Budget mensuel non trouvé");

        var entriesByType = budget.MonthlyEntries.GroupBy(e => e.Type);
        var incomes = entriesByType.FirstOrDefault(g => g.Key == EntryType.Income) ?? Enumerable.Empty<MonthlyEntry>();
        var charges = entriesByType.FirstOrDefault(g => g.Key == EntryType.Charge) ?? Enumerable.Empty<MonthlyEntry>();

        budget.RemainingBudget = CalculateMonthlyRemainingBudget(incomes, charges, budget.Expenses);

        await context.SaveChangesAsync();
        return budget.RemainingBudget;
    }
}