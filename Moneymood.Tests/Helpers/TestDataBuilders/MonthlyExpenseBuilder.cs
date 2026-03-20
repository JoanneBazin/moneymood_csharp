using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Models;

namespace MoneyMood.Tests.Helpers.TestDataBuilders;

public class MonthlyExpenseBuilder
{
    private Guid _monthlyBudgetId = Guid.NewGuid();
    private string _name = "Expense";
    private decimal _amount = 10;
    
    

    public MonthlyExpenseBuilder ForBudget(Guid monthlyBudgetId)
    {
        _monthlyBudgetId = monthlyBudgetId;
        return this;
    }

    public MonthlyExpenseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    public MonthlyExpenseBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public async Task<ExpenseData> BuildAndSaveAsync(AppDbContext db)
    {
        var expense = new Expense
        {
            MonthlyBudgetId = _monthlyBudgetId,
            Name = _name,
            Amount = _amount,
            WeekNumber = 1,
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync();
        return new ExpenseData(expense.Id, expense.Name, expense.Amount);
    }

    public MonthlyExpenseRequest Build()
    {
        return new MonthlyExpenseRequest
        {
            Name = _name,
            Amount = _amount,
            WeekNumber = 1,
        };
    }

    public record ExpenseData(Guid Id, string Name, decimal Amount);
}