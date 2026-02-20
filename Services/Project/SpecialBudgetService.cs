using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Category;
using MoneyMood.Dtos.Project;
using MoneyMood.Exceptions;
using MoneyMood.Models;
using MoneyMood.Services.Budget;

namespace MoneyMood.Services.Project;

public class SpecialBudgetService(AppDbContext context, IBudgetCalculationService calculationService) : ISpecialBudgetService
{
    public async Task<SpecialBudgetResponse> CreateSpecialBudgetAsync(Guid userId, SpecialBudgetRequest request)
    {
        var budget = new SpecialBudget
        {
            UserId = userId,
            Name = request.Name,
            TotalBudget = request.TotalBudget,
            RemainingBudget = request.TotalBudget
        };

        context.SpecialBudgets.Add(budget);
        await context.SaveChangesAsync();

        return MapToBudgetResponse(budget);
    }

    public async Task<IEnumerable<SpecialBudgetListResponse>> GetAllSpecialBudgetsAsync(Guid userId)
    {
        return await context.SpecialBudgets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new SpecialBudgetListResponse { Id = b.Id, Name = b.Name, CreatedAt = b.CreatedAt })
            .ToListAsync();
    }

    public async Task<SpecialBudgetResponse> GetSpecialBudgetByIdAsync(Guid userId, Guid budgetId)
    {
        var budget = await context.SpecialBudgets
            .Include(b => b.Expenses)
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId)
            ?? throw ApiException.NotFound("Budget introuvable");
        
        return MapToBudgetResponse(budget);
    }

    public async Task<SpecialBudgetResponse> UpdateSpecialBudgetAsync(Guid userId, Guid budgetId, SpecialBudgetRequest request)
    {
        var budget = await context.SpecialBudgets
        .Include(b => b.Expenses)
        .Include(b => b.Categories)
        .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId)
        ?? throw ApiException.NotFound("Budget introuvable");  

        budget.Name = request.Name;  
        budget.TotalBudget = request.TotalBudget; 
        budget.RemainingBudget = calculationService.CalculateSpecialRemainingBudget(budget.TotalBudget, budget.Expenses);
        
        await context.SaveChangesAsync();

        return MapToBudgetResponse(budget);  
    }

    public async Task<DeletedSpecialBudgetResponse> DeleteSpecialBudgetAsync(Guid userId, Guid budgetId)
    {
        var budget = await context.SpecialBudgets
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId)
                ?? throw ApiException.NotFound("Budget non trouvé ou vous n'avez pas les droits d'accès");
        
        context.SpecialBudgets.Remove(budget);
        await context.SaveChangesAsync();
        return new DeletedSpecialBudgetResponse
        {
            Id = budget.Id,
        };
    }

    private static SpecialBudgetResponse MapToBudgetResponse(SpecialBudget budget) => new()
    {
        Id = budget.Id,
        Name = budget.Name,
        TotalBudget = budget.TotalBudget,
        RemainingBudget = budget.RemainingBudget,
        CreatedAt = budget.CreatedAt,
        Expenses = budget.Expenses
            .Where(e => e.SpecialCategoryId == null)
            .Select(e => new SpecialExpenseResponse {Id = e.Id, Name = e.Name, Amount = e.Amount, Cashed = e.Cashed, SpecialCategoryId = e.SpecialCategoryId, CreatedAt = e.CreatedAt}),
        Categories = budget.Categories
            .Select(c => new CategoryResponse {Id = c.Id, Name = c.Name, SpecialBudgetId = c.SpecialBudgetId, Expenses = c.Expenses
                .Select(e => new SpecialExpenseResponse {Id = e.Id, Name = e.Name, Amount = e.Amount, Cashed = e.Cashed, SpecialCategoryId = e.SpecialCategoryId, CreatedAt = e.CreatedAt})})
    };
}