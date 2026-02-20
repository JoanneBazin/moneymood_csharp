namespace MoneyMood.Dtos.Project;

public class SpecialBudgetRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
}