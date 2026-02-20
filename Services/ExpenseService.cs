using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Expense;
using MoneyMood.Enums;
using MoneyMood.Exceptions;
using MoneyMood.Models;
using MoneyMood.Services.Budget;

namespace MoneyMood.Services;

public class ExpenseService(AppDbContext context, IBudgetCalculationService calculationService) : IExpenseService
{
    public async Task<ExpenseOperationResponse<IEnumerable<ExpenseResponse>>> CreateExpensesAsync(Guid userId, Guid budgetId, BudgetType budgetType, ICollection<ExpenseRequest> request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId, budgetType);

        var isMonthly = budgetType == BudgetType.Monthly;

        var expenses = request.Select(r => new Expense
        {
            Name = r.Name,
            Amount = r.Amount,
            WeekNumber = isMonthly ? r.WeekNumber : null,
            MonthlyBudgetId = isMonthly ? budgetId : null,
            SpecialBudgetId = !isMonthly ? budgetId : null
        }).ToList();

        context.Expenses.AddRange(expenses);
        await context.SaveChangesAsync();

        var data = expenses.Select(e => MapToExpenseResponse(e));

        return isMonthly 
        ? await MapToMonthlyExpenseResponseAsync(budgetId, data) 
        : await MapToSpecialExpenseResponseAsync(budgetId, data);
    }

    public async Task<ExpenseOperationResponse<ExpenseResponse>>UpdateExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType, ExpenseRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId, budgetType);

        var isMonthly = budgetType == BudgetType.Monthly;

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && (isMonthly ? e.MonthlyBudgetId == budgetId : e.SpecialBudgetId == budgetId));

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        expense.Name = request.Name;
        expense.Amount = request.Amount;
        await context.SaveChangesAsync();

        return isMonthly 
        ? await MapToMonthlyExpenseResponseAsync(budgetId, MapToExpenseResponse(expense)) 
        : await MapToSpecialExpenseResponseAsync(budgetId, MapToExpenseResponse(expense));
    }

    public async Task<ExpenseResponse>UpdateExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType, UpdateExpenseValidationRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId, budgetType);

        var isMonthly = budgetType == BudgetType.Monthly;

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && (isMonthly ? e.MonthlyBudgetId == budgetId : e.SpecialBudgetId == budgetId));

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");
        
        expense.Cashed = request.Cashed;
        expense.CashedAt = request.Cashed ? DateTime.UtcNow : null;
        await context.SaveChangesAsync();

        return MapToExpenseResponse(expense);
    }

    public async Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType)
    {
        await EnsureBudgetAccessAsync(userId, budgetId, budgetType);

        var isMonthly = budgetType == BudgetType.Monthly;

        var expense = await context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && (isMonthly ? e.MonthlyBudgetId == budgetId : e.SpecialBudgetId == budgetId));

        if (expense is null)
            throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync();

        var data = new DeletedExpenseResponse { Id = expense.Id};
        return isMonthly 
        ? await MapToMonthlyExpenseResponseAsync(budgetId, data) 
        : await MapToSpecialExpenseResponseAsync(budgetId, data);
    }

    private async Task EnsureBudgetAccessAsync(Guid userId, Guid budgetId, BudgetType budgetType)
    {
        var hasAccess = budgetType == BudgetType.Monthly 
        ? await context.MonthlyBudgets.AnyAsync(b => b.Id == budgetId && b.UserId == userId)
        : await context.SpecialBudgets.AnyAsync(b => b.Id == budgetId && b.UserId == userId);

        if (!hasAccess)
            throw ApiException.NotFound("Budget mensuel non trouvé ou vous n'avez pas les droits d'accès");
    }

    private static ExpenseResponse MapToExpenseResponse(Expense entry) => new()
    {
        
            Id = entry.Id,
            Name = entry.Name,
            Amount = entry.Amount,
            WeekNumber = entry.WeekNumber,
            Cashed = entry.Cashed

    };

    private async Task<ExpenseOperationResponse<T>> MapToMonthlyExpenseResponseAsync<T>(Guid budgetId, T data)
    {
        var remainingBudget = await calculationService.UpdateMonthlyRemainingBudgetAsync(budgetId, context);
        return new ExpenseOperationResponse<T>
        {
            Data = data,
            RemainingBudget = remainingBudget,
        };
    }

    private async Task<ExpenseOperationResponse<T>> MapToSpecialExpenseResponseAsync<T>(Guid budgetId, T data)
    {
        // var remainingBudget = await calculationService.UpdateMonthlyRemainingBudgetAsync(budgetId, context);
        return new ExpenseOperationResponse<T>
        {
            Data = data,
            RemainingBudget = 0,
        };
    }
}