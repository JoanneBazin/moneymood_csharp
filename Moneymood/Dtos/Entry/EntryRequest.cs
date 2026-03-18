using MoneyMood.Interfaces;

namespace MoneyMood.Dtos.Entry;

public class EntryRequest : IHasAmount
{
    private string _name = string.Empty;
    public string Name 
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty;
    }
    public decimal Amount { get; set; }
}