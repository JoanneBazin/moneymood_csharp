using MoneyMood.Dtos.Expense;
using MoneyMood.Enums;

namespace MoneyMood.Services;

public interface IExpenseService
{
   Task<ExpenseOperationResponse<IEnumerable<ExpenseResponse>>> CreateExpensesAsync(Guid userId, Guid budgetId, BudgetType budgetType, ICollection<ExpenseRequest> request);
   Task<ExpenseOperationResponse<ExpenseResponse>>UpdateExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType, ExpenseRequest request);
   Task<ExpenseResponse>UpdateExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType, UpdateExpenseValidationRequest request);
   Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, BudgetType budgetType);
}