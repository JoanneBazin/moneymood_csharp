using Microsoft.EntityFrameworkCore;
using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Exceptions;
using MoneyMood.Models;

namespace MoneyMood.Services.Budget;

public class MonthlyBudgetService(AppDbContext context, IBudgetCalculationService calculationService) : IMonthlyBudgetService
{
    public async Task<MonthlyBudgetResponse> CreateMonthlyBudgetAsync(Guid userId, CreateMonthlyBudgetRequest request)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var remainingBudget = calculationService.CalculateMonthlyRemainingBudget(request.Incomes, request.Charges);

            var entries = request.Incomes
                .Select(i => new MonthlyEntry { Name = i.Name, Amount = i.Amount, Type = EntryType.Income })
                .Concat(request.Charges
                .Select(c => new MonthlyEntry { Name = c.Name, Amount = c.Amount, Type = EntryType.Charge }))
                .ToList();

            var budget = new MonthlyBudget
            {
                UserId = userId,
                Month = request.Month,
                Year = request.Year,
                IsCurrent = request.IsCurrent,
                RemainingBudget = remainingBudget,
                WeeklyBudget = calculationService.CalculateWeeklyBudget(remainingBudget, request.NumberOfWeeks),
                NumberOfWeeks = request.NumberOfWeeks,
                MonthlyEntries = entries

            };

            context.MonthlyBudgets.Add(budget);
            await context.SaveChangesAsync();   

            if (request.IsCurrent)
            {
                await context.MonthlyBudgets
                    .Where(b => b.UserId == userId && b.Id != budget.Id)
                    .ExecuteUpdateAsync(b => b.SetProperty(x => x.IsCurrent, false));
            }

            await transaction.CommitAsync();
            return MapToResponse(budget);
        }

        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync();
            throw ApiException.Conflict("Un budget mensuel pour ce mois existe déjà");
        } catch
        {
            await transaction.RollbackAsync();
            throw;
        }  
    }

    public async Task<HistoryResponse> GetMonthlyBudgetByDateAsync(Guid userId, int month, int year)
    {
        var budget = await context.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Year == year && b.Month == month) ?? throw ApiException.NotFound("Budget mensuel introuvable");
        
        return MapHistoryToResponse(budget);
    }

    public async Task<MonthlyBudgetResponse?> GetCurrentMonthlyBudgetAsync(Guid userId)
    {
        var budget = await context.MonthlyBudgets
            .Include(b => b.MonthlyEntries)
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.IsCurrent);
        
        if (budget is null) return null;
        return MapToResponse(budget);
    }

    public async Task<MonthlyBudgetResponse> GetMonthlyBudgetByIdAsync(Guid userId, Guid budgetId)
    {
         var budget = await context.MonthlyBudgets
            .Include(b => b.MonthlyEntries)
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId)
            ?? throw ApiException.NotFound("Budget mensuel introuvable");

        return MapToResponse(budget);
    }


    public async Task<IEnumerable<HistoryResponse>> GetLastBudgetsAsync(Guid userId)
    {
        return await context.MonthlyBudgets
            .Where(b => b.UserId == userId && !b.IsCurrent)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .Take(6)
            .Select(b => new HistoryResponse
            {
                Id = b.Id,
                Year = b.Year,
                Month = b.Month,
                RemainingBudget = b.RemainingBudget
            })
            .ToListAsync();
        
    }

    public async Task<MonthlyBudgetResponse> UpdateMonthlyBudgetStatusAsync(Guid userId, Guid budgetId, UpdateBudgetStatusRequest request)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var budget = await context.MonthlyBudgets
                .Include(b => b.MonthlyEntries)
                .Include(b => b.Expenses)
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId)
                ?? throw ApiException.NotFound("Budget non trouvé ou vous n'avez pas les droits d'accès");

            budget.IsCurrent = request.IsCurrent;
            await context.SaveChangesAsync();   

            if (request.IsCurrent)
            {
                await context.MonthlyBudgets
                    .Where(b => b.UserId == userId && b.Id != budget.Id)
                    .ExecuteUpdateAsync(b => b.SetProperty(x => x.IsCurrent, false));
            }

            await transaction.CommitAsync();
            return MapToResponse(budget);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }  
    }

    public async Task<DeletedBudgetResponse> DeleteMonthlyBudgetAsync(Guid userId, Guid budgetId)
    {
        var budget = await context.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId)
                ?? throw ApiException.NotFound("Budget non trouvé ou vous n'avez pas les droits d'accès");
        
        context.MonthlyBudgets.Remove(budget);
        await context.SaveChangesAsync();
        return new DeletedBudgetResponse
        {
            Id = budget.Id,
            IsCurrent = budget.IsCurrent
        };
    }

    private static MonthlyBudgetResponse MapToResponse(MonthlyBudget budget) => new()
    {
        Id = budget.Id,
        Month = budget.Month,
        Year = budget.Year,
        IsCurrent = budget.IsCurrent,
        RemainingBudget = budget.RemainingBudget,
        WeeklyBudget = budget.WeeklyBudget,
        NumberOfWeeks = budget.NumberOfWeeks,
        Incomes = budget.MonthlyEntries
            .Where(e => e.Type == EntryType.Income)
            .Select(e => new EntryResponse {Id = e.Id, Name = e.Name, Amount = e.Amount}),
        Charges = budget.MonthlyEntries
            .Where(e => e.Type == EntryType.Charge)
            .Select(e => new EntryResponse {Id = e.Id, Name = e.Name, Amount = e.Amount}),
            Expenses = []
    };
    private static HistoryResponse MapHistoryToResponse(MonthlyBudget budget) => new()
    {
        Id = budget.Id,
        Month = budget.Month,
        Year = budget.Year,
        RemainingBudget = budget.RemainingBudget,
    };
}