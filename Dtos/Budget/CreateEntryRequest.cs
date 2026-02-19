using MoneyMood.Interfaces;

namespace MoneyMood.Dtos.Budget;

public class CreateEntryRequest : IHasAmount
{
   public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}