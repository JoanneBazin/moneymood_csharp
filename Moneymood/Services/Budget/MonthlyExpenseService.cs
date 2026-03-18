using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Expense;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.Budget;

public class MonthlyExpenseService(AppDbContext context, IBudgetCalculationService calculationService) : IMonthlyExpenseService
{
    public async Task<ExpenseOperationResponse<IEnumerable<MonthlyExpenseResponse>>> CreateMonthlyExpensesAsync(Guid userId, Guid budgetId, ICollection<MonthlyExpenseRequest> request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expenses = request.Select(r => new Expense
        {
            Name = r.Name,
            Amount = r.Amount,
            WeekNumber = r.WeekNumber,
            MonthlyBudgetId = budgetId,
        }).ToList();

        context.Expenses.AddRange(expenses);
        await context.SaveChangesAsync();

        var data = expenses.Select(e => MapToExpenseResponse(e));

        return await MapToResponseAsync(budgetId, data);
    }

    public async Task<ExpenseOperationResponse<MonthlyExpenseResponse>>UpdateMonthlyExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, ExpenseRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.MonthlyBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        expense.Name = request.Name;
        expense.Amount = request.Amount;
        await context.SaveChangesAsync();

        return await MapToResponseAsync(budgetId, MapToExpenseResponse(expense));
    }

    public async Task<MonthlyExpenseResponse>UpdateMonthlyExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, UpdateExpenseValidationRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.MonthlyBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");
        
        expense.Cashed = request.Cashed;
        expense.CashedAt = request.Cashed ? DateTime.UtcNow : null;
        await context.SaveChangesAsync();

        return MapToExpenseResponse(expense);
    }

    public async Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteMonthlyExpenseAsync(Guid userId, Guid budgetId, Guid expenseId)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.MonthlyBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync();

        var data = new DeletedExpenseResponse { Id = expense.Id};
        return await MapToResponseAsync(budgetId, data);
    }

    private async Task EnsureBudgetAccessAsync(Guid userId, Guid budgetId)
    {
        var hasAccess = await context.MonthlyBudgets.AnyAsync(b => b.Id == budgetId && b.UserId == userId);

        if (!hasAccess)
            throw ApiException.NotFound("Budget mensuel non trouvé ou vous n'avez pas les droits d'accès");
    }

    private static MonthlyExpenseResponse MapToExpenseResponse(Expense entry) => new()
    {
        
            Id = entry.Id,
            Name = entry.Name,
            Amount = entry.Amount,
            WeekNumber = entry.WeekNumber!.Value,
            Cashed = entry.Cashed

    };

    private async Task<ExpenseOperationResponse<T>> MapToResponseAsync<T>(Guid budgetId, T data)
    {
        var remainingBudget = await calculationService.UpdateMonthlyRemainingBudgetAsync(budgetId, context);
        return new ExpenseOperationResponse<T>
        {
            Data = data,
            RemainingBudget = remainingBudget,
        };
    }
}