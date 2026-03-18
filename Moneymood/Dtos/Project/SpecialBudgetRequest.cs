namespace MoneyMood.Dtos.Project;

public class SpecialBudgetRequest
{
    private string _name = string.Empty;
    public string Name 
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty;
    }
    public decimal TotalBudget { get; set; }
}