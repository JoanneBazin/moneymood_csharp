namespace MoneyMood.Dtos.Auth;

public class SessionInfo
{
    
    public Guid UserId { get; set; }
    public Guid Session { get; set; }
    public bool ShouldRefresh { get; set; }
}