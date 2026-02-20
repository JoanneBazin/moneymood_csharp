using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Expense;
using MoneyMood.Dtos.Project;
using MoneyMood.Exceptions;
using MoneyMood.Models;
using MoneyMood.Services.Budget;

namespace MoneyMood.Services.Project;

public class SpecialExpenseService(AppDbContext context, IBudgetCalculationService calculationService) : ISpecialExpenseService
{
    public async Task<ExpenseOperationResponse<IEnumerable<SpecialExpenseResponse>>> CreateSpecialExpensesAsync(Guid userId, Guid budgetId, ICollection<SpecialExpenseRequest> request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expenses = request.Select(r => new Expense
        {
            Name = r.Name,
            Amount = r.Amount,
            SpecialBudgetId = budgetId,
            SpecialCategoryId = r.SpecialCategoryId
        }).ToList();

        context.Expenses.AddRange(expenses);
        await context.SaveChangesAsync();

        var data = expenses.Select(e => MapToExpenseResponse(e));

        return await MapToResponseAsync(budgetId, data);
    }

    public async Task<ExpenseOperationResponse<SpecialExpenseResponse>>UpdateSpecialExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, SpecialExpenseRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.SpecialBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        expense.Name = request.Name;
        expense.Amount = request.Amount;
        expense.SpecialCategoryId = request.SpecialCategoryId;
        await context.SaveChangesAsync();

        return await MapToResponseAsync(budgetId, MapToExpenseResponse(expense));
    }

    public async Task<SpecialExpenseResponse>UpdateSpecialExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, UpdateExpenseValidationRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.SpecialBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");
        
        expense.Cashed = request.Cashed;
        expense.CashedAt = request.Cashed ? DateTime.UtcNow : null;
        await context.SaveChangesAsync();

        return MapToExpenseResponse(expense);
    }

    public async Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteSpecialExpenseAsync(Guid userId, Guid budgetId, Guid expenseId)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.SpecialBudgetId == budgetId);

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync();

        var data = new DeletedExpenseResponse { Id = expense.Id};
        return await MapToResponseAsync(budgetId, data);
    }

    private async Task EnsureBudgetAccessAsync(Guid userId, Guid budgetId)
    {
        var hasAccess = await context.SpecialBudgets.AnyAsync(b => b.Id == budgetId && b.UserId == userId);

        if (!hasAccess)
            throw ApiException.NotFound("Budget mensuel non trouvé ou vous n'avez pas les droits d'accès");
    }

    private static SpecialExpenseResponse MapToExpenseResponse(Expense entry) => new()
    {
        
            Id = entry.Id,
            Name = entry.Name,
            Amount = entry.Amount,
            Cashed = entry.Cashed,
            SpecialCategoryId = entry.SpecialCategoryId,
            CreatedAt = entry.CreatedAt

    };

    private async Task<ExpenseOperationResponse<T>> MapToResponseAsync<T>(Guid budgetId, T data)
    {
        var remainingBudget = await calculationService.UpdateSpecialRemainingBudgetAsync(budgetId, context);
        return new ExpenseOperationResponse<T>
        {
            Data = data,
            RemainingBudget = remainingBudget,
        };
    }
}