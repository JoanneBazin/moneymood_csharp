using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moneymood.Tests.Shared;
using MoneyMood.Dtos.Budget;
using MoneyMood.Tests.Base;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Helpers.TestDataBuilders;

namespace Moneymood.Tests.Integration.Budget;

public class MonthlyEntriesControllerTests(DatabaseFixture dbFixture) : IntegrationTestBase(dbFixture)
{
    [Theory]
    [InlineData(EntryType.Income, "incomes")]
    [InlineData(EntryType.Charge, "charges")]
    public async Task CreateMonthlyEntries_ReturnsOkAndEntriesWithRemainingBudget(EntryType type,
    string routeSegment)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var entriesRequest = new[] 
        { 
            new MonthlyEntryBuilder().WithName($"Test {type}").Build() 
        };

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{budget.Id}/{routeSegment}", entriesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<MonthlyEntryOperationResponse<List<EntryResponse>>>();
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data[0].Name.Should().Be(entriesRequest[0].Name);
        result.RemainingBudget.Should().Be(type == EntryType.Income 
        ? budget.RemainingBudget + result.Data[0].Amount
        : budget.RemainingBudget - result.Data[0].Amount);

        using var dbCheck = GetDbContext();
        var entryInDb = await dbCheck.MonthlyEntries
            .FirstOrDefaultAsync(e => e.Id == result.Data[0].Id && 
            e.MonthlyBudgetId == budget.Id);

        entryInDb.Should().NotBeNull();
        entryInDb.Type.Should().Be(type);
        entryInDb.Name.Should().Be(entriesRequest[0].Name);
    }

    [Theory]
    [InlineData(EntryType.Income, "incomes")]
    [InlineData(EntryType.Charge, "charges")]
    public async Task CreateMonthlyEntries_WithInvalidBudgetId_ReturnsNotFound(EntryType type,
    string routeSegment)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        var entriesRequest = new[] 
        { 
            new MonthlyEntryBuilder().WithName($"Test {type}").Build() 
        };
        var randomId = Guid.NewGuid();

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{randomId}/{routeSegment}", entriesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("Budget mensuel non trouvé");

        using var dbCheck = GetDbContext();
        var entryInDb = await dbCheck.MonthlyEntries.FirstOrDefaultAsync(e => e.Name == entriesRequest[0].Name);
        entryInDb.Should().BeNull();
    }

    [Theory]
    [InlineData(EntryType.Income, "incomes")]
    [InlineData(EntryType.Charge, "charges")]
    public async Task CreateMonthlyEntries_WithInvalidData_ReturnsBadRequest(EntryType type,
    string routeSegment)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var entriesRequest = new[] 
        { 
            new { name = "", amount = 10 }, 
            new { name = $"Test {type}", amount = 0 } 
        };

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{budget.Id}/{routeSegment}", entriesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("Le nom est requis");
        error.Error.Should().Contain("Veuillez saisir un montant positif valide");

        using var dbCheck = GetDbContext();
        var entryInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Name == entriesRequest[0].name && e.MonthlyBudgetId == budget.Id);
        entryInDb.Should().BeNull();
    }

    [Theory]
    [InlineData(EntryType.Income, "incomes")]
    [InlineData(EntryType.Charge, "charges")]
    public async Task UpdateMonthlyEntries_ReturnsOkAndExpenseWithRemainingBudget(EntryType type,
    string routeSegment)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var entry = await new MonthlyEntryBuilder()
            .ForBudget(budget.Id)
            .WithType(type)
            .BuildAndSaveAsync(db);
        
        var updateRequest = new { name = $"Updated {type}", amount = 200 };

        var response = await Client.PutAsJsonAsync($"/api/monthly-budgets/{budget.Id}/{routeSegment}/{entry.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<MonthlyEntryOperationResponse<EntryResponse>>();
        result.Should().NotBeNull();
        result.Data.Name.Should().Be(updateRequest.name);
        result.Data.Amount.Should().Be(updateRequest.amount);

        using var dbCheck = GetDbContext();
        var entryInDb = await dbCheck.MonthlyEntries
            .FirstOrDefaultAsync(e => e.Id == result.Data.Id && 
            e.MonthlyBudgetId == budget.Id);

        entryInDb.Should().NotBeNull();
        entryInDb.Type.Should().Be(type);
        entryInDb.Name.Should().Be(updateRequest.name);
        entryInDb.Amount.Should().Be(updateRequest.amount);
    }

    [Theory]
    [InlineData("incomes")]
    [InlineData("charges")]
    public async Task DeleteMonthlyEntries_ReturnsOkAndExpenseWithRemainingBudget(
    string routeSegment)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var entry = await new MonthlyEntryBuilder()
            .ForBudget(budget.Id)
            .BuildAndSaveAsync(db);
        
        var response = await Client.DeleteAsync($"/api/monthly-budgets/{budget.Id}/{routeSegment}/{entry.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<MonthlyEntryOperationResponse<EntryResponse>>();
        result.Should().NotBeNull();
        result.Data.Id.Should().Be(entry.Id);

        using var dbCheck = GetDbContext();
        var entryInDb = await dbCheck.MonthlyEntries
            .FirstOrDefaultAsync(e => e.Id == result.Data.Id && 
            e.MonthlyBudgetId == budget.Id);

        entryInDb.Should().BeNull();
    }

}