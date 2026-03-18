using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Category;
using MoneyMood.Dtos.Project;
using MoneyMood.Exceptions;
using MoneyMood.Models;
using MoneyMood.Services.Budget;

namespace MoneyMood.Services.Project;

public class SpecialCategoryService(AppDbContext context, IBudgetCalculationService calculationService) : ISpecialCategoryService
{
    public async Task<CategoryResponse> CreateSpecialCategoryAsync(Guid userId, Guid budgetId, CategoryRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var category = new SpecialCategory
        {
            Name = request.Name,
            SpecialBudgetId = budgetId
        };
        context.SpecialCategories.Add(category);
        await context.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<CategoryResponse> UpdateSpecialCategoryAsync(Guid userId, Guid budgetId, Guid categoryId, CategoryRequest request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var category = await context.SpecialCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.SpecialBudgetId == budgetId)
            ?? throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        category.Name = request.Name;
        await context.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<DeletedCategoryResponse> DeleteSpecialCategoryAsync(Guid userId, Guid budgetId, Guid categoryId)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var category = await context.SpecialCategories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(e => e.Id == categoryId && e.SpecialBudgetId == budgetId)
            ?? throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        context.SpecialCategories.Remove(category);
        await context.SaveChangesAsync();

        return new DeletedCategoryResponse
        {
            Id = category.Id,
            Expenses = category.Expenses
                .Select(e => new SpecialExpenseResponse {Id = e.Id, Name = e.Name, Amount = e.Amount, Cashed = e.Cashed, SpecialCategoryId = e.SpecialCategoryId, CreatedAt = e.CreatedAt})
        };
    }

    public async Task<DeletedCategoryCascadeResponse> DeleteSpecialCategoryOnCascadeAsync(Guid userId, Guid budgetId, Guid categoryId)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var category = await context.SpecialCategories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(e => e.Id == categoryId && e.SpecialBudgetId == budgetId)
            ?? throw ApiException.NotFound("Dépense non trouvée ou non liée au budget");

        context.SpecialCategories.Remove(category);
        context.Expenses.RemoveRange(category.Expenses);
        await context.SaveChangesAsync();

        return new DeletedCategoryCascadeResponse
        {
            RemainingBudget = await calculationService.UpdateSpecialRemainingBudgetAsync(budgetId, context)
        };
    }

    private async Task EnsureBudgetAccessAsync(Guid userId, Guid budgetId)
    {
        var hasAccess = await context.SpecialBudgets.AnyAsync(b => b.Id == budgetId && b.UserId == userId);

        if (!hasAccess)
            throw ApiException.NotFound("Budget mensuel non trouvé ou vous n'avez pas les droits d'accès");
    }

    private static CategoryResponse MapToResponse(SpecialCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        SpecialBudgetId = category.SpecialBudgetId,
        Expenses = category.Expenses
            .Select(e => new SpecialExpenseResponse {Id = e.Id, Name = e.Name, Amount = e.Amount, Cashed = e.Cashed, SpecialCategoryId = e.SpecialCategoryId, CreatedAt = e.CreatedAt})
    };
}