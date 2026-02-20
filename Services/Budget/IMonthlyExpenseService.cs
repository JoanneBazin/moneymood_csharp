using MoneyMood.Dtos.Budget;
using MoneyMood.Dtos.Expense;

namespace MoneyMood.Services.Budget;

public interface IMonthlyExpenseService
{
   Task<ExpenseOperationResponse<IEnumerable<MonthlyExpenseResponse>>> CreateMonthlyExpensesAsync(Guid userId, Guid budgetId, ICollection<MonthlyExpenseRequest> request);
   Task<ExpenseOperationResponse<MonthlyExpenseResponse>>UpdateMonthlyExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, ExpenseRequest request);
   Task<MonthlyExpenseResponse>UpdateMonthlyExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, UpdateExpenseValidationRequest request);
   Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteMonthlyExpenseAsync(Guid userId, Guid budgetId, Guid expenseId);
}