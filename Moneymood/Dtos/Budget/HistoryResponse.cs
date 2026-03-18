namespace MoneyMood.Dtos.Budget;

public class HistoryResponse
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal RemainingBudget { get; set; }
}