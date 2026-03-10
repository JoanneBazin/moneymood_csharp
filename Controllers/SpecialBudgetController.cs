using Microsoft.AspNetCore.Mvc;
using MoneyMood.Dtos.Category;
using MoneyMood.Dtos.Expense;
using MoneyMood.Dtos.Project;
using MoneyMood.Services.Project;

namespace MoneyMood.Controllers;

[ApiController]
[Route("api/special-budgets")]
public class SpecialBudgetController(
    ISpecialBudgetService budgetService,
    ISpecialCategoryService categoryService,
    ISpecialExpenseService expenseService
) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<SpecialBudgetResponse>> CreateSpecialBudget(SpecialBudgetRequest request)
    {
        var result = await budgetService.CreateSpecialBudgetAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetSpecialBudgetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecialBudgetListResponse>>> GetAllSpecialBudgets()
    {
        return Ok(await budgetService.GetAllSpecialBudgetsAsync(GetUserId()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpecialBudgetResponse>> GetSpecialBudgetById(Guid id)
    {
        return Ok(await budgetService.GetSpecialBudgetByIdAsync(GetUserId(), id));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SpecialBudgetResponse>> UpdateSpecialBudget(Guid id, SpecialBudgetRequest request)
    {
        return Ok(await budgetService.UpdateSpecialBudgetAsync(GetUserId(), id, request));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedSpecialBudgetResponse>> DeleteSpecialBudget(Guid id)
    {
        return Ok(await budgetService.DeleteSpecialBudgetAsync(GetUserId(), id));
    }

    [HttpPost("{id}/categories")]
    public async Task<ActionResult<CategoryResponse>> CreateSpecialCategory(Guid id, CategoryRequest request)
    {
        return Ok(await categoryService.CreateSpecialCategoryAsync(GetUserId(), id, request));
    }

    [HttpPatch("{id}/categories/{categoryId}")]
    public async Task<ActionResult<CategoryResponse>> UpdateSpecialCategory(Guid id, Guid categoryId, CategoryRequest request)
    {
        return Ok(await categoryService.UpdateSpecialCategoryAsync(GetUserId(), id, categoryId, request));
    }

    [HttpDelete("{id}/categories/{categoryId}")]
    public async Task<ActionResult<DeletedCategoryResponse>> DeleteSpecialCategory(Guid id, Guid categoryId)
    {
        return Ok(await categoryService.DeleteSpecialCategoryAsync(GetUserId(), id, categoryId));
    }

    [HttpDelete("{id}/categories/{categoryId}/cascade")]
    public async Task<ActionResult<DeletedCategoryCascadeResponse>> DeleteSpecialCategoryOnCascade(Guid id, Guid categoryId)
    {
        return Ok(await categoryService.DeleteSpecialCategoryOnCascadeAsync(GetUserId(), id, categoryId));
    }

    [HttpPost("{id}/expenses")]
    public async Task<ActionResult<ExpenseOperationResponse<IEnumerable<SpecialExpenseResponse>>>> CreateSpecialExpense(Guid id, ICollection<SpecialExpenseRequest> request)
    {
        return Ok(await expenseService.CreateSpecialExpensesAsync(GetUserId(), id, request));
    }

    [HttpPut("{id}/expenses/{expenseId}")]
    public async Task<ActionResult<ExpenseOperationResponse<SpecialExpenseResponse>>> UpdateSpecialExpense(Guid id, Guid expenseId, SpecialExpenseRequest request)
    {
        return Ok(await expenseService.UpdateSpecialExpenseAsync(GetUserId(), id, expenseId, request));
    }

    [HttpPatch("{id}/expenses/{expenseId}/cashed")]
    public async Task<ActionResult<SpecialExpenseResponse>> UpdateSpecialExpenseValidation(Guid id, Guid expenseId, UpdateExpenseValidationRequest request)
    {
        return Ok(await expenseService.UpdateSpecialExpenseValidationAsync(GetUserId(), id, expenseId, request));
    }

    [HttpDelete("{id}/expenses/{expenseId}")]
    public async Task<ActionResult<ExpenseOperationResponse<DeletedExpenseResponse>>> DeleteSpecialExpense(Guid id, Guid expenseId)
    {
        return Ok(await expenseService.DeleteSpecialExpenseAsync(GetUserId(), id, expenseId));
    }
}