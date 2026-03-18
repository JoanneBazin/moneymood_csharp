using MoneyMood.Dtos.Expense;
using MoneyMood.Dtos.Project;

namespace MoneyMood.Services.Project;

public interface ISpecialExpenseService
{
   Task<ExpenseOperationResponse<IEnumerable<SpecialExpenseResponse>>> CreateSpecialExpensesAsync(Guid userId, Guid budgetId, ICollection<SpecialExpenseRequest> request);
   Task<ExpenseOperationResponse<SpecialExpenseResponse>>UpdateSpecialExpenseAsync(Guid userId, Guid budgetId, Guid expenseId, SpecialExpenseRequest request);
   Task<SpecialExpenseResponse>UpdateSpecialExpenseValidationAsync(Guid userId, Guid budgetId, Guid expenseId, UpdateExpenseValidationRequest request);
   Task<ExpenseOperationResponse<DeletedExpenseResponse>>DeleteSpecialExpenseAsync(Guid userId, Guid budgetId, Guid expenseId);
}