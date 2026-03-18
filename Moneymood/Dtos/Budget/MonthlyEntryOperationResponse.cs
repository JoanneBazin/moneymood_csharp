namespace MoneyMood.Dtos.Budget;

public class MonthlyEntryOperationResponse<T>
{
    public T Data { get; set; } = default!;
    public decimal WeeklyBudget { get; set; }
    public decimal RemainingBudget { get; set; }
}