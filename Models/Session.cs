namespace MoneyMood.Models;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}