namespace MoneyMood.Dtos.Expense;

public class ExpenseOperationResponse<T>
{
    public T Data { get; set; } = default!;
    public decimal RemainingBudget { get; set; }
}