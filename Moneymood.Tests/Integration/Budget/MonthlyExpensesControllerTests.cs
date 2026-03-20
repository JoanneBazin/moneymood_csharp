using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moneymood.Tests.Shared;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Expense;
using MoneyMood.Tests.Base;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Helpers.TestDataBuilders;

namespace Moneymood.Tests.Integration.Budget;

public class MonthlyExpensesControllerTests(DatabaseFixture dbFixture) : IntegrationTestBase(dbFixture)
{
    [Fact]
    public async Task CreateMonthlyExpenses_ReturnsOkAndExpensesWithRemainingBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var expensesRequest = new[] { new MonthlyExpenseBuilder().Build() };

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{budget.Id}/expenses", expensesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ExpenseOperationResponse<List<MonthlyExpenseResponse>>>();
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data[0].Name.Should().Be(expensesRequest[0].Name);
        result.RemainingBudget.Should().Be(budget.RemainingBudget - result.Data[0].Amount);

        using var dbCheck = GetDbContext();
        var expenseInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Id == result.Data[0].Id && e.MonthlyBudgetId == budget.Id);
        expenseInDb.Should().NotBeNull();

    }

    [Fact]
    public async Task CreateMonthlyExpenses_WithInvalidBudgetId_ReturnsNotFound()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        var expensesRequest = new[] { new MonthlyExpenseBuilder().Build() };
        var randomId = Guid.NewGuid();

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{randomId}/expenses", expensesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("Budget mensuel non trouvé");

        using var dbCheck = GetDbContext();
        var expenseInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Name == expensesRequest[0].Name);
        expenseInDb.Should().BeNull();
    }

    [Theory]
    [InlineData("income", 0, "Veuillez saisir un montant positif valide")]
    [InlineData("", 10, "Le nom est requis")]
    public async Task CreateMonthlyExpenses_WithInvalidData_ReturnsBadRequest(
        string name, 
        decimal amount,
        string expectedError)
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var expensesRequest = new[] { 
            new MonthlyExpenseBuilder().WithName(name).WithAmount(amount).Build() 
        };

        var response = await Client.PostAsJsonAsync($"/api/monthly-budgets/{budget.Id}/expenses", expensesRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain(expectedError);

        using var dbCheck = GetDbContext();
        var expenseInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Name == expensesRequest[0].Name && e.MonthlyBudgetId == budget.Id);
        expenseInDb.Should().BeNull();
    }

    [Fact]
    public async Task UpdateMonthlyExpense_ReturnsOkAndExpenseWithRemainingBudget()
    {
        await ResetDb();
        
        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var expense = await new MonthlyExpenseBuilder()
            .ForBudget(budget.Id)
            .BuildAndSaveAsync(db);
        var updateRequest = new { name = "Updated expense", amount = 20 };

        var response = await Client.PutAsJsonAsync($"/api/monthly-budgets/{budget.Id}/expenses/{expense.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ExpenseOperationResponse<MonthlyExpenseResponse>>();
        result.Should().NotBeNull();
        result.Data.Name.Should().Be(updateRequest.name);
        result.Data.Amount.Should().Be(updateRequest.amount);

        using var dbCheck = GetDbContext();
        var expenseInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Id == expense.Id && e.MonthlyBudgetId == budget.Id);
        expenseInDb.Should().NotBeNull();
        expenseInDb.Name.Should().Be(updateRequest.name);
        expenseInDb.Amount.Should().Be(updateRequest.amount);
    }

    [Fact]
    public async Task DeleteMonthlyExpense_ReturnsOkAndIdWithRemainingBudget()
    {
        await ResetDb();

        var authHelper = CreateAuthHelper();
        var user = await authHelper.CreateAuthenticatedUserAsync();

        using var db = GetDbContext();
        var budget = await new MonthlyBudgetBuilder()
            .ForUser(user.Id)
            .BuildAndSaveAsync(db);

        var expense = await new MonthlyExpenseBuilder()
            .ForBudget(budget.Id)
            .BuildAndSaveAsync(db);

        var response = await Client.DeleteAsync($"/api/monthly-budgets/{budget.Id}/expenses/{expense.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ExpenseOperationResponse<DeletedExpenseResponse>>();
        result.Should().NotBeNull();
        result.Data.Id.Should().Be(expense.Id);

        using var dbCheck = GetDbContext();
        var expenseInDb = await dbCheck.Expenses.FirstOrDefaultAsync(e => e.Id == expense.Id && e.MonthlyBudgetId == budget.Id);
        expenseInDb.Should().BeNull();
    }
}