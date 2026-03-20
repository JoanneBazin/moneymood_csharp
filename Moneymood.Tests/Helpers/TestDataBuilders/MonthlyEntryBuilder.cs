using MoneyMood.Data;
using MoneyMood.Dtos.Entry;
using MoneyMood.Models;

namespace MoneyMood.Tests.Helpers.TestDataBuilders;

public class MonthlyEntryBuilder
{
    private Guid _budgetId = Guid.NewGuid();
    private string _name = "Entry";
    private decimal _amount = 10;
    private EntryType _type = EntryType.Income;

    public MonthlyEntryBuilder ForBudget(Guid budgetId)
    {
        _budgetId = budgetId;
        return this;
    }

    public MonthlyEntryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    public MonthlyEntryBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }
    public MonthlyEntryBuilder WithType(EntryType type)
    {
        _type = type;
        return this;
    }

    public async Task<MonthlyEntry> BuildAndSaveAsync(AppDbContext db)
    {
        var entry = new MonthlyEntry
        {
            MonthlyBudgetId = _budgetId,
            Name = _name,
            Amount = _amount,
            Type = _type,
        };

        db.MonthlyEntries.Add(entry);
        await db.SaveChangesAsync();
        return entry;
    }

    public EntryRequest Build()
    {
        return new EntryRequest
        {
            Name = _name,
            Amount = _amount,
        };
    }
}