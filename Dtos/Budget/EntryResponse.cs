namespace MoneyMood.Dtos.Budget;

public class EntryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}