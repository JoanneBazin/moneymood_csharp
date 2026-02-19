using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Budget;
using MoneyMood.Services.Budget;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/monthly-budgets")]
public class MonthlyBudgetController(IMonthlyBudgetService budgetService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<MonthlyBudgetResponse>> CreateMonthlyBudget(CreateMonthlyBudgetRequest request)
    {
        var userId = GetUserId();
        var result = await budgetService.CreateMonthlyBudgetAsync(userId, request);
        return CreatedAtAction(nameof(GetMonthlyBudgetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<HistoryResponse>> GetMonthlyBudgetByDate(int month, int year)
    {
        var userId = GetUserId();
        var result = await budgetService.GetMonthlyBudgetByDateAsync(userId, month, year);
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<ActionResult<MonthlyBudgetResponse>> GetCurrentMonthlyBudget()
    {
        var userId = GetUserId();
        var result = await budgetService.GetCurrentMonthlyBudgetAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlyBudgetResponse>> GetMonthlyBudgetById(Guid id)
    {
        var userId = GetUserId();
        var result = await budgetService.GetMonthlyBudgetByIdAsync(userId, id);
        return Ok(result);
    }


    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<HistoryResponse>>> GetLastBudgets()
    {
        var userId = GetUserId();
        var result = await budgetService.GetLastBudgetsAsync(userId);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<IEnumerable<HistoryResponse>>> UpdateMonthlyBudgetStatus(Guid id, UpdateBudgetStatusRequest request)
    {
        var userId = GetUserId();
        var result = await budgetService.UpdateMonthlyBudgetStatusAsync(userId, id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<HistoryResponse>>> DeleteMonthlyBudget(Guid id)
    {
        var userId = GetUserId();
        var result = await budgetService.DeleteMonthlyBudgetAsync(userId, id);
        return Ok(result);
    }
}