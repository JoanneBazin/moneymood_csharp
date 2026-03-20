using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moneymood.Tests.Shared;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Tests.Base;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Helpers.TestDataBuilders;

namespace Moneymood.Tests.Integration.Budget;

public class MonthlyBudgetControllerTests(DatabaseFixture dbFixture) : IntegrationTestBase(dbFixture)
{
    [Fact]
    public async Task CreateMonthlyBudget_ReturnsCreatedAndBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        var budgetRequest = new MonthlyBudgetBuilder()
            .WithCurrentStatus(true)
            .WithIncomes([new EntryRequest { Name = "Income", Amount = 1000}])
            .WithCharges([new EntryRequest { Name = "Charge", Amount = 600}])
            .Build();

        var response = await Client.PostAsJsonAsync("/api/monthly-budgets", budgetRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var budget = await response.Content.ReadFromJsonAsync<MonthlyBudgetResponse>();
        budget.Should().NotBeNull();
        budget.Month.Should().Be(budgetRequest.Month);
        budget.Year.Should().Be(budgetRequest.Year);
        budget.IsCurrent.Should().Be(budgetRequest.IsCurrent);
        budget.RemainingBudget.Should().Be(400);
        budget.WeeklyBudget.Should().Be(100);

        using var dbCheck = GetDbContext();
        var budgetInDb = await dbCheck.MonthlyBudgets.FirstOrDefaultAsync(b => b.Year == budgetRequest.Year && b.Month == budgetRequest.Month && b.UserId == user.Id);
        budgetInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateMonthlyBudget_WithoutAuth_ReturnsUnauthorized()
    {
        await ResetDb();

        var budgetRequest = new MonthlyBudgetBuilder()
            .Build();

        var response = await Client.PostAsJsonAsync("/api/monthly-budgets", budgetRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(13, 2026, 4, "Le mois doit être compris entre 1 et 12")]
    [InlineData(1, 1000, 4, "L'année doit être supérieure ou égale à 2025")]
    [InlineData(1, 2026, 6, "Nombre de semaines invalide")]
    public async Task CreateMonthlyBudget_WithInvalidData_ReturnsBadRequest(
        int month,
        int year,
        int numberOfWeeks,
        string expectedError
    )
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        var budgetRequest = new MonthlyBudgetBuilder()
            .WithMonth(month)
            .WithYear(year)
            .WithNumberOfWeeks(numberOfWeeks)
            .Build();

        var response = await Client.PostAsJsonAsync("/api/monthly-budgets", budgetRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain(expectedError);

        using var dbCheck = GetDbContext();
        var budgetInDb = await dbCheck.MonthlyBudgets.FirstOrDefaultAsync(b => b.Year == budgetRequest.Year && b.Month == budgetRequest.Month && b.UserId == user.Id);
        budgetInDb.Should().BeNull();
    }

    [Fact]
    public async Task CreateMonthlyBudget_WithDuplicateMonthYear_ReturnsConflict()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var existantBudgetRequest = new MonthlyBudgetBuilder()
            .Build();

        var response = await Client.PostAsJsonAsync("/api/monthly-budgets", existantBudgetRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        using var dbCheck = GetDbContext();
        var budgetInDb = await dbCheck.MonthlyBudgets.FirstOrDefaultAsync(b => b.Year == existantBudgetRequest.Year && b.Month == existantBudgetRequest.Month && b.UserId == user.Id);
        budgetInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCurrentBudget_ReturnsOkAndBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var currentBudget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .WithCurrentStatus(true)
            .BuildAndSaveAsync(db);

        var response = await Client.GetAsync("/api/monthly-budgets/current");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var budget = await response.Content.ReadFromJsonAsync<MonthlyBudgetResponse>();
        budget.Should().NotBeNull();
        budget.Id.Should().Be(currentBudget.Id);
        budget.IsCurrent.Should().Be(true);
    }

    [Fact]
    public async Task GetBudgetById_ReturnsOkAndBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var currentBudget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var response = await Client.GetAsync($"/api/monthly-budgets/{currentBudget.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var budget = await response.Content.ReadFromJsonAsync<MonthlyBudgetResponse>();
        budget.Should().NotBeNull();
        budget.Id.Should().Be(currentBudget.Id);
        budget.Month.Should().Be(currentBudget.Month);
        budget.Year.Should().Be(currentBudget.Year);
    }

    [Fact]
    public async Task GetBudgetHistory_ReturnsOkAndArrayOfBudgets()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .WithMonth(1)
            .WithCurrentStatus(false)
            .BuildAndSaveAsync(db);
        await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .WithMonth(2)
            .WithCurrentStatus(false)
            .BuildAndSaveAsync(db);

        var response = await Client.GetAsync("/api/monthly-budgets/history");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var budgets = await response.Content.ReadFromJsonAsync<List<HistoryResponse>>();
        budgets.Should().NotBeNull();
        budgets.Should().HaveCount(2);
        budgets[0].Month.Should().Be(2);
    }

    [Fact]
    public async Task UpdateBudgetStatus_ReturnsOkAndUpdatedBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .WithCurrentStatus(false)
            .BuildAndSaveAsync(db);
        

        var response = await Client.PatchAsJsonAsync($"/api/monthly-budgets/{budget.Id}", new { isCurrent = !budget.IsCurrent });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedBudget = await response.Content.ReadFromJsonAsync<MonthlyBudgetResponse>();
        updatedBudget.Should().NotBeNull();
        updatedBudget.IsCurrent.Should().Be(!budget.IsCurrent);
    }

    [Fact]
    public async Task DeleteMonthlyBudget_ReturnsOkAndIdWithStatus()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);
        

        var response = await Client.DeleteAsync($"/api/monthly-budgets/{budget.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedBudget = await response.Content.ReadFromJsonAsync<DeletedBudgetResponse>();
        updatedBudget.Should().NotBeNull();
        updatedBudget.Id.Should().Be(budget.Id);
        updatedBudget.IsCurrent.Should().Be(budget.IsCurrent);

        using var dbCheck = GetDbContext();
        var budgetInDb = await dbCheck.MonthlyBudgets.FirstOrDefaultAsync(b => b.Id == budget.Id);
        budgetInDb.Should().BeNull();
    }
}