using MoneyMood.Interfaces;

namespace MoneyMood.Dtos.Entry;

public class EntryRequest : IHasAmount
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}