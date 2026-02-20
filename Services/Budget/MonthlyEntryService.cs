using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.Budget;

public class MonthlyEntryService(AppDbContext context, IBudgetCalculationService calculationService) : IMonthlyEntryService
{
    public async Task<MonthlyEntryOperationResponse<IEnumerable<EntryResponse>>> CreateMonthlyEntriesAsync(Guid userId, Guid budgetId, EntryType type, ICollection<EntryRequest> request)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var entries = request.Select(r => new MonthlyEntry
        {
            Name = r.Name,
            Amount = r.Amount,
            Type = type,
            MonthlyBudgetId = budgetId
        }).ToList();

        context.MonthlyEntries.AddRange(entries);
        await context.SaveChangesAsync();

        var data = entries.Select(e => MapToEntryResponse(e));

        return await MapToResponseAsync(budgetId, data);
    }

    public async Task<MonthlyEntryOperationResponse<EntryResponse>> UpdateMonthlyEntryAsync(Guid userId, Guid budgetId, Guid entryId, EntryRequest request) 
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var entry = await context.MonthlyEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.MonthlyBudgetId == budgetId);

        if (entry is null)
            throw ApiException.NotFound("Entrée non trouvée ou non liée au budget");

        entry.Name = request.Name;
        entry.Amount = request.Amount;
        await context.SaveChangesAsync();

        return await MapToResponseAsync(budgetId, MapToEntryResponse(entry));

    }

    public async Task<MonthlyEntryOperationResponse<DeletedEntryResponse>> DeleteMonthlyEntryAsync(Guid userId, Guid budgetId, Guid entryId)
    {
        await EnsureBudgetAccessAsync(userId, budgetId);

        var entry = await context.MonthlyEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.MonthlyBudgetId == budgetId);

        if (entry is null)
            throw ApiException.NotFound("Entrée non trouvée ou non liée au budget");
        
        context.MonthlyEntries.Remove(entry);
        await context.SaveChangesAsync();

        var data = new DeletedEntryResponse { Id = entry.Id};

        return await MapToResponseAsync(budgetId, data);
    }


    private async Task EnsureBudgetAccessAsync(Guid userId, Guid budgetId)
    {
        var hasAccess = await context.MonthlyBudgets
            .AnyAsync(b => b.Id == budgetId && b.UserId == userId);

        if (!hasAccess)
            throw ApiException.NotFound("Budget mensuel non trouvé ou vous n'avez pas les droits d'accès");
    }

    private static EntryResponse MapToEntryResponse(MonthlyEntry entry) => new()
    {
        
            Id = entry.Id,
            Name = entry.Name,
            Amount = entry.Amount
    };

    private async Task<MonthlyEntryOperationResponse<T>> MapToResponseAsync<T>(Guid budgetId, T data)
    {
        var weeklyBudget = await calculationService.UpdateWeeklyBudgetAsync(budgetId, context);
        var remainingBudget = await calculationService.UpdateMonthlyRemainingBudgetAsync(budgetId, context);
        return new MonthlyEntryOperationResponse<T>
        {
            Data = data,
            RemainingBudget = remainingBudget,
            WeeklyBudget = weeklyBudget
        };
    }
}