using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Entry;
using MoneyMood.Dtos.Expense;
using MoneyMood.Services.Budget;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/monthly-budgets")]
public class MonthlyBudgetController(
    IMonthlyBudgetService budgetService, 
    IMonthlyEntryService entryService,
    IMonthlyExpenseService expenseService
    ) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<MonthlyBudgetResponse>> CreateMonthlyBudget(CreateMonthlyBudgetRequest request)
    {
        var result = await budgetService.CreateMonthlyBudgetAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetMonthlyBudgetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<HistoryResponse>> GetMonthlyBudgetByDate(int month, int year)
    {
        return Ok(await budgetService.GetMonthlyBudgetByDateAsync(GetUserId(), month, year));
    }

    [HttpGet("current")]
    public async Task<ActionResult<MonthlyBudgetResponse>> GetCurrentMonthlyBudget()
    {
        return Ok(await budgetService.GetCurrentMonthlyBudgetAsync(GetUserId()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlyBudgetResponse>> GetMonthlyBudgetById(Guid id)
    {
        return Ok(await budgetService.GetMonthlyBudgetByIdAsync(GetUserId(), id));
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<HistoryResponse>>> GetLastBudgets()
    {
        return Ok(await budgetService.GetLastBudgetsAsync(GetUserId()));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<MonthlyBudgetResponse>> UpdateMonthlyBudgetStatus(Guid id, UpdateBudgetStatusRequest request)
    {
        return Ok(await budgetService.UpdateMonthlyBudgetStatusAsync(GetUserId(), id, request));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedBudgetResponse>> DeleteMonthlyBudget(Guid id)
    {
        return Ok(await budgetService.DeleteMonthlyBudgetAsync(GetUserId(), id));
    }

    [HttpPost("{id}/incomes")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<IEnumerable<EntryResponse>>>> CreateMonthlyIncomes(Guid id, ICollection<EntryRequest> request)
    {
        return Ok(await entryService.CreateMonthlyEntriesAsync(GetUserId(), id, EntryType.Income, request));
    }

    [HttpPut("{id}/incomes/{incomeId}")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<EntryResponse>>> UpdateMonthlyIncome(Guid id, Guid incomeId, EntryRequest request)
    {
        return Ok(await entryService.UpdateMonthlyEntryAsync(GetUserId(), id, incomeId, request));
    }

    [HttpDelete("{id}/incomes/{incomeId}")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<DeletedEntryResponse>>> DeleteMonthlyIncome(Guid id, Guid incomeId)
    {
        return Ok(await entryService.DeleteMonthlyEntryAsync(GetUserId(), id, incomeId));
    }

    [HttpPost("{id}/charges")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<IEnumerable<EntryResponse>>>> CreateMonthlyCharges(Guid id, ICollection<EntryRequest> request)
    {
        return Ok(await entryService.CreateMonthlyEntriesAsync(GetUserId(), id, EntryType.Charge, request));
    }

    [HttpPut("{id}/charges/{chargeId}")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<EntryResponse>>> UpdateMonthlyCharge(Guid id, Guid chargeId, EntryRequest request)
    {
        return Ok(await entryService.UpdateMonthlyEntryAsync(GetUserId(), id, chargeId, request));
    }

    [HttpDelete("{id}/charges/{chargeId}")]
    public async Task<ActionResult<MonthlyEntryOperationResponse<DeletedEntryResponse>>> DeleteMonthlyCharge(Guid id, Guid chargeId)
    {
        return Ok(await entryService.DeleteMonthlyEntryAsync(GetUserId(), id, chargeId));
    }

    [HttpPost("{id}/expenses")]
    public async Task<ActionResult<ExpenseOperationResponse<IEnumerable<MonthlyExpenseResponse>>>> CreateMonthlyExpenses(Guid id, ICollection<MonthlyExpenseRequest> request)
    {
        return Ok(await expenseService.CreateMonthlyExpensesAsync(GetUserId(), id, request));
    }

    [HttpPut("{id}/expenses/{expenseId}")]
    public async Task<ActionResult<ExpenseOperationResponse<MonthlyExpenseResponse>>> UpdateMonthlyExpense(Guid id, Guid expenseId, MonthlyExpenseRequest request)
    {
        return Ok(await expenseService.UpdateMonthlyExpenseAsync(GetUserId(), id, expenseId, request));
    }

    [HttpPatch("{id}/expenses/{expenseId}/cashed")]
    public async Task<ActionResult<MonthlyExpenseResponse>> UpdateMonthlyExpenseValidation(Guid id, Guid expenseId, UpdateExpenseValidationRequest request)
    {
        return Ok(await expenseService.UpdateMonthlyExpenseValidationAsync(GetUserId(), id, expenseId, request));
    }

    [HttpDelete("{id}/expenses/{expenseId}")]
    public async Task<ActionResult<ExpenseOperationResponse<DeletedExpenseResponse>>> DeleteMonthlyExpense(Guid id, Guid expenseId)
    {
        return Ok(await expenseService.DeleteMonthlyExpenseAsync(GetUserId(), id, expenseId));
    }
}