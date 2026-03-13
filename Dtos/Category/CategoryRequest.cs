namespace MoneyMood.Dtos.Category;

public class CategoryRequest
{
   private string _name = string.Empty;
    public string Name 
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty;
    }
}