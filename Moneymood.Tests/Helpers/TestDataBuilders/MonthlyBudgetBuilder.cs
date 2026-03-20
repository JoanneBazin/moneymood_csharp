using MoneyMood.Data;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Models;

namespace MoneyMood.Tests.Helpers.TestDataBuilders;

public class MonthlyBudgetBuilder
{
    private Guid _userId = Guid.NewGuid();
    private int _month = 1;
    private int _year = 2026;
    private int _numberOfWeeks = 4;
    private bool _isCurrent = false;
    private ICollection<EntryRequest> _incomes = [];
    private ICollection<EntryRequest> _charges = [];

    public MonthlyBudgetBuilder ForUser(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public MonthlyBudgetBuilder WithMonth(int month)
    {
        _month = month;
        return this;
    }
    public MonthlyBudgetBuilder WithYear(int year)
    {
        _year = year;
        return this;
    }

    public MonthlyBudgetBuilder WithCurrentStatus(bool isCurrent)
    {
        _isCurrent = isCurrent;
        return this;
    }

    public MonthlyBudgetBuilder WithNumberOfWeeks(int numberOfWeeks)
    {
        _numberOfWeeks = numberOfWeeks;
        return this;
    }

    public MonthlyBudgetBuilder WithIncomes(ICollection<EntryRequest> incomes)
    {
        _incomes = incomes;
        return this;
    }

    public MonthlyBudgetBuilder WithCharges(ICollection<EntryRequest> charges)
    {
        _charges = charges;
        return this;
    }

    public async Task<MonthlyBudget> BuildAndSaveAsync(AppDbContext db)
    {
        var budget = new MonthlyBudget
        {
            UserId = _userId,
            Month = _month,
            Year = _year,
            IsCurrent = _isCurrent,
            NumberOfWeeks = _numberOfWeeks,
            RemainingBudget = 0,
            WeeklyBudget = 0,
            MonthlyEntries = []
        };

        db.MonthlyBudgets.Add(budget);
        await db.SaveChangesAsync();
        return budget;
    }

    public CreateMonthlyBudgetRequest Build()
    {
        return new CreateMonthlyBudgetRequest
        {
            Month = _month,
            Year = _year,
            IsCurrent = _isCurrent,
            NumberOfWeeks = _numberOfWeeks,
            Incomes = _incomes,
            Charges = _charges
        };
    }
}