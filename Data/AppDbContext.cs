using Microsoft.EntityFrameworkCore;
using MoneyMood.Models;

namespace MoneyMood.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<FixedEntry> FixedEntries { get; set; }
    public DbSet<MonthlyEntry> MonthlyEntries { get; set; }
    public DbSet<MonthlyBudget> MonthlyBudgets { get; set; }
    public DbSet<SpecialBudget> SpecialBudgets { get; set; }
    public DbSet<SpecialCategory> SpecialCategories { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<FixedEntry>()
            .Property(e => e.Amount)
            .HasPrecision(10, 2);
        
        modelBuilder.Entity<MonthlyEntry>()
            .Property(e => e.Amount)
            .HasPrecision(10, 2);
        modelBuilder.Entity<MonthlyBudget>()
            .Property(e => e.RemainingBudget)
            .HasPrecision(10, 2);
        modelBuilder.Entity<MonthlyBudget>()
            .Property(e => e.WeeklyBudget)
            .HasPrecision(10, 2);
        modelBuilder.Entity<SpecialBudget>()
            .Property(e => e.TotalBudget)
            .HasPrecision(10, 2);
        modelBuilder.Entity<SpecialBudget>()
            .Property(e => e.RemainingBudget)
            .HasPrecision(10, 2);
        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<MonthlyBudget>()
            .HasIndex(mb => new { mb.UserId, mb.Month, mb.Year })
            .IsUnique();

        modelBuilder.Entity<FixedEntry>()
            .Property(e => e.Type)
            .HasConversion<string>();
        modelBuilder.Entity<MonthlyEntry>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.MonthlyBudget)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.MonthlyBudgetId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.SpecialBudget)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.SpecialBudgetId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.SpecialCategory)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.SpecialCategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<SpecialCategory>()
            .HasOne(sc => sc.SpecialBudget)
            .WithMany(sc => sc.Categories)
            .HasForeignKey(sc => sc.SpecialBudgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}