namespace MoneyMood.Dtos.Auth;

public class SessionInfo
{
    
    public string UserId { get; set; } = string.Empty;
    public string Session { get; set; } = string.Empty;
    public bool ShouldRefresh { get; set; }
}