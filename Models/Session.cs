namespace MoneyMood.Models;

public class Session
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}